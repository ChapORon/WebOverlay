#pragma once

#include <boost/asio.hpp>
#include "Logger.hpp"

namespace Network
{
	class User
	{
		friend class Server;
	private:
		uint16_t m_ID{ 0 };
		std::shared_ptr<Logger> m_Logger;
		std::atomic_bool m_Closed;
		boost::asio::io_context& m_IOContext;
		boost::asio::ip::tcp::socket m_Socket;

	private:
		void Read();

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
				m_Logger->DebugLog("BoostNetworkUser", function, std::forward<t_Args>(args)...);
		}

	public:
		User(uint16_t, const std::shared_ptr<Logger> &, boost::asio::io_context&, boost::asio::ip::tcp::socket);
		void Send(const std::string &);
		void Close();
		inline uint16_t GetID() const { return m_ID; }
		~User();
	};
}