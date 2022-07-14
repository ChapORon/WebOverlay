#include "Concurrency/ThreadSystem.hpp"

#include "Logger.hpp"

namespace Concurrency
{
    ThreadSystem::ThreadSystem(const std::shared_ptr<Logger> &logger) : m_CreatorThreadID(std::this_thread::get_id()), m_Logger(logger) {}

    TaskResult ThreadSystem::TaskThreadLoop(int id)
    {
        if (!m_Running.load())
            return TaskResult::Fail;
        Log(LogType::Verbose, "[Thread ", id, "] Waiting to start tasks");
        Task* task;
        {
            std::unique_lock<std::mutex> lock(m_TaskMutex);
            m_TaskCondition.wait(lock, [=] { return !m_Tasks.empty() || !m_Running.load(); });
            if (m_Running.load())
            {
                Log(LogType::Verbose, "[Thread ", id, "] Starting task");
                task = m_Tasks.front();
                m_Tasks.pop();
            }
            else
                return TaskResult::Fail;
        }
        if (task->Update() == TaskResult::NeedUpdate)
            AddTask(task);
        else
            delete(task);
        return TaskResult::NeedUpdate;
    }

    TaskResult ThreadSystem::ShedulerThreadLoop(int id)
    {
        if (!m_Running.load())
            return TaskResult::Fail;
        //Log(LogType::Verbose, "[Thread ", id, "] Checking scheduled tasks");
        std::unique_lock<std::mutex> lock(m_ShedulerMutex);
        for (const auto& pair : m_ScheduledTasks)
        {

        }
        return TaskResult::NeedUpdate;
    }

    void ThreadSystem::AddTask(Task* taskToAdd)
    {
        if (m_Running.load())
        {
            {
                std::unique_lock<std::mutex> lock(m_TaskMutex);
                m_Tasks.push(taskToAdd);
            }
            m_TaskCondition.notify_one();
        }
    }

    void ThreadSystem::Await()
    {
        if (std::this_thread::get_id() == m_CreatorThreadID)
        {
            std::unique_lock<std::mutex> lock(m_ThreadsMutex);
            m_ThreadsCondition.wait(lock, [=] { return !m_Running.load(); });
            Clean();
        }
        else
            Log(LogType::Error, "Cannot await ThreadSystem outside of creator thread");
    }

    void ThreadSystem::Start()
    {
        if (!m_Running.load())
        {
            m_Running.store(true);
            int maximumThread = std::thread::hardware_concurrency() / 2;
            for (int i = 0; i < maximumThread; ++i)
            {
                if (i == 0)
                    m_Threads.emplace_back(std::make_pair(new Thread(m_Logger, i, [this](int id) { return ShedulerThreadLoop(id); }), ThreadType::SchedulerThread));
                else
                    m_Threads.emplace_back(std::make_pair(new Thread(m_Logger, i, [this](int id) { return TaskThreadLoop(id); }), ThreadType::TaskThread));
                m_Threads[i].first->Start();
            }
        }
    }

    void ThreadSystem::Stop()
    {
        if (m_Running.load())
        {
            m_OnStopCallback();
            m_Running.store(false);
            m_TaskCondition.notify_all();
            m_ThreadsCondition.notify_all();
        }
    }

    void ThreadSystem::Clean()
    {
        Log(LogType::Standard, "Stopping thread system");
        for (std::pair<Thread*, ThreadType>& pair : m_Threads)
        {
            Thread* thread = pair.first;
            thread->Stop();
            delete(thread);
        }
        m_Threads.clear();
        while (!m_Tasks.empty())
        {
            delete(m_Tasks.front());
            m_Tasks.pop();
        }
        m_ScheduledTasks.clear();
    }

    ThreadSystem::~ThreadSystem()
    {
        Stop();
        if (m_Running.load())
            Clean();
    }
}