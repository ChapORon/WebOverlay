#pragma once

#include <functional>
#include <memory>
#include <queue>
#include "Concurrency/Task.hpp"
#include "Concurrency/Thread.hpp"

namespace Concurrency
{
    class ThreadSystem
    {
    private:
        enum class ThreadType : short
        {
            TaskThread,
            SchedulerThread
        };

    private:
        std::thread::id m_CreatorThreadID;
        std::atomic_bool m_Running;
        std::queue<Task*> m_Tasks;
        std::mutex m_TaskMutex;
        std::mutex m_ThreadsMutex;
        std::mutex m_ShedulerMutex;
        std::condition_variable m_TaskCondition;
        std::condition_variable m_ThreadsCondition;
        std::vector<std::pair<Thread*, ThreadType>> m_Threads;
        std::vector<std::pair<size_t, Task*>> m_ScheduledTasks;
        std::function<void()> m_OnStopCallback;
        std::shared_ptr<Logger> m_Logger;

    private:
        TaskResult TaskThreadLoop(int);
        TaskResult ShedulerThreadLoop(int);
        void Clean();
        void AddTask(Task*);

        template <class... t_Args>
        void Log(LogType logType, t_Args &&... args) const
        {
            if (m_Logger)
                m_Logger->Log(logType, std::forward<t_Args>(args)...);
        }

    public:
        ThreadSystem(const std::shared_ptr<Logger> &);
        void Start();
        void SetOnStopCallback(const std::function<void()> &onStopCallback) { m_OnStopCallback = onStopCallback; }
        void Stop();
        void Await();

        template <typename t_Object>
        void AddTask(t_Object* obj, TaskResult(t_Object::* fct)()) { AddTask(new FunctorTask<t_Object>(obj, fct)); }
        template <typename t_Task, typename ...t_Arg>
        void AddTask(t_Arg&&... args) { AddTask(new t_Task(std::forward<t_Arg>(args)...)); }
        void AddTask(const std::function<TaskResult()> &taskToAdd) { AddTask(new CallableTask(taskToAdd)); }

        ~ThreadSystem();
    };
}