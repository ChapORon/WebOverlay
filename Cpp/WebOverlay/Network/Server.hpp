#pragma once

#include <boost/asio.hpp>
#include <unordered_map>
#include "Logger.hpp"
#include "Network/IDGenerator.hpp"

namespace Concurrency
{
	class ThreadSystem;
}

namespace Network
{
	class User;
	class Server
	{
	private:
		bool m_WhitelistEnable{ false };
		bool m_IsStarted{ false };
		int m_Port;
		std::shared_ptr<Concurrency::ThreadSystem> m_ThreadSystem;
		std::shared_ptr<Logger> m_Logger;
		std::unordered_set<std::string> m_WhitelistedIP;
		std::atomic_bool m_IsClosing;
		std::atomic_bool m_Closed;
		boost::asio::io_context m_IOContext;
		boost::asio::ip::tcp::acceptor m_Acceptor;
		std::unordered_map<uint16_t, std::shared_ptr<User>> m_Sessions;
		IDGenerator m_IDGenerator;

	private:
		void CloseSession(uint16_t);
		void AcceptSession();
		bool IsIPWhitelisted(const std::string&);

		template <class... t_Args>
		void Log(LogType logType, t_Args &&... args) const
		{
			if (m_Logger)
				m_Logger->Log(logType, std::forward<t_Args>(args)...);
		}

		template <class... t_Args>
		void DebugLog(const std::string& function, t_Args &&... args) const
		{
			if (m_Logger)
				m_Logger->DebugLog("Server", function, std::forward<t_Args>(args)...);
		}

	public:
		Server(int, const std::string &, const std::shared_ptr<Concurrency::ThreadSystem> &, const std::shared_ptr<Logger> &);
		bool Start();
		void Stop();
	};
}