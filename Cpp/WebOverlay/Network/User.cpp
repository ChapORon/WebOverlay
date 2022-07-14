#include "Network/User.hpp"
#include "Logger.hpp"

namespace Network
{
	User::User(uint16_t id, const std::shared_ptr<Logger> &logger, boost::asio::io_context& ioContext, boost::asio::ip::tcp::socket socket) :
		m_ID(id),
		m_Closed(false),
		m_Logger(logger),
		m_IOContext(ioContext),
		m_Socket(std::move(socket))
	{
		Read();
	}

	void User::Read()
	{
		boost::asio::streambuf buffer(std::numeric_limits<std::size_t>::max());
		boost::asio::async_read_until(m_Socket, buffer, "\r\n\r\n", [this, &buffer](const boost::system::error_code& ec, std::size_t bytes_transferred) {
			if (!ec)
			{
				std::string command{ boost::asio::buffers_begin(buffer.data()), boost::asio::buffers_begin(buffer.data()) + bytes_transferred - 4 /*Delimiter size*/};
			}
			else if (!m_Closed.load())
				Close();
		});
	}

	void User::Send(const std::string& message)
	{
		boost::asio::async_write(m_Socket, boost::asio::buffer(message.c_str(), message.size()), [this](const boost::system::error_code& ec, size_t) {
			if (ec && !m_Closed.load())
				Close();
		});
	}

	void User::Close()
	{
		if (!m_Closed.load())
		{
			Log(LogType::Standard, '[', GetID(), "] Closing network session");
			boost::asio::post(m_IOContext, [this]() {
				m_Socket.close();
			});
			m_Closed.store(true);
		}
	}

	User::~User()
	{
		Close();
	}
}