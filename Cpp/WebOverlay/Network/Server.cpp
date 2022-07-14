#include "Network/Server.hpp"

#include <fstream>
#include "Concurrency/ThreadSystem.hpp"
#include "Logger.hpp"
#include "Network/User.hpp"

namespace Network
{
	Server::Server(int port,
		const std::string &whitelistFile,
		const std::shared_ptr<Concurrency::ThreadSystem> &threadSystem,
		const std::shared_ptr<Logger> &logger) :
		m_WhitelistEnable(!whitelistFile.empty()),
		m_Port(port),
		m_ThreadSystem(threadSystem),
		m_Logger(logger),
		m_IsClosing(false),
		m_Closed(false),
		m_Acceptor(m_IOContext, boost::asio::ip::tcp::endpoint(boost::asio::ip::tcp::v4(), port))
	{
		if (!whitelistFile.empty())
		{
			std::ifstream file(whitelistFile);
			if (file)
			{
				std::string str;
				while (std::getline(file, str))
					m_WhitelistedIP.insert(str);
			}
			else
				m_WhitelistEnable = false;
		}
	}

	bool Server::IsIPWhitelisted(const std::string& ip)
	{
		if (ip == "127.0.0.1" || !m_WhitelistEnable)
			return true;
		return m_WhitelistedIP.find(ip) != m_WhitelistedIP.end();
	}

	void Server::CloseSession(uint16_t id)
	{
		Log(LogType::Standard, "Closing session with ID: ", id);
		m_Sessions.erase(m_Sessions.find(id));
	}

	void Server::AcceptSession()
	{
		m_Acceptor.async_accept([this](boost::system::error_code ec, boost::asio::ip::tcp::socket socket) {
			if (!ec)
			{
				std::string clientIP = socket.remote_endpoint().address().to_string();
				Log(LogType::Standard, "New connection on IP ", clientIP);
				if (IsIPWhitelisted(clientIP))
				{
					if (std::shared_ptr<User> networkUser = std::make_shared<User>(m_IDGenerator.GenerateID(), m_Logger, m_IOContext, std::move(socket)))
					{
						uint16_t id = networkUser->GetID();
						Log(LogType::Standard, "New session opened with ID: ", id);
						m_Sessions[id] = networkUser;
					}
				}
			}
			AcceptSession();
		});
	}

	bool Server::Start()
	{
		m_ThreadSystem->AddTask([=]() {
			Log(LogType::Standard, "Server created on port ", m_Port);
			AcceptSession();
			m_IOContext.run();
			Log(LogType::Standard, "Network stopped");
			return Concurrency::TaskResult::Success;
		});
		return true;
	}

	void Server::Stop()
	{
		if (!m_Closed.load())
		{
			boost::asio::post(m_IOContext, [this]() {
				Log(LogType::Standard, "Closing server");
				m_IsClosing.store(true);
				for (const auto& pair : m_Sessions)
					m_IDGenerator.FreeID(pair.first);
				m_Sessions.clear();
				Log(LogType::Standard, "Stopping network");
				m_IOContext.stop();
				m_Closed.store(true);
			});
		}
	}
}