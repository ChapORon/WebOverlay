#include "Logger.hpp"

#include <chrono>
#include <filesystem>
#include <fstream>
#include <iostream>
#include "StringUtils.hpp"

Logger::Logger(const std::string &format, bool isVerbose, bool logInFile, const std::string& logFilePath) :
	m_Format(format),
	m_IsVerbose(isVerbose),
	m_LogInFile(logInFile),
	m_LogFilePath(logFilePath) {}

void Logger::LogStr(LogType logType, const std::string& str) const
{
	if (logType == LogType::Verbose && !m_IsVerbose)
		return;
	std::string message = str;
	std::string log = m_Format;
	StringUtils::ReplaceAll(log, "{msg}", str);
	std::chrono::time_point<std::chrono::system_clock> now = std::chrono::system_clock::now();
	size_t milliSeconds = (std::chrono::duration_cast<std::chrono::milliseconds>(now.time_since_epoch()) % 1000).count();
	std::time_t time = std::chrono::system_clock::to_time_t(now);
	std::tm* localTime = std::localtime(&time);
	StringUtils::ReplaceAll(log, "{y}", std::to_string(localTime->tm_year + 1900));
	//month
	if (localTime->tm_mon >= 9)
		StringUtils::ReplaceAll(log, "{M}", std::to_string(localTime->tm_mon + 1));
	else
		StringUtils::ReplaceAll(log, "{M}", "0" + std::to_string(localTime->tm_mon + 1));
	//day
	if (localTime->tm_mday >= 10)
		StringUtils::ReplaceAll(log, "{d}", std::to_string(localTime->tm_mday));
	else
		StringUtils::ReplaceAll(log, "{d}", "0" + std::to_string(localTime->tm_mday));
	//hour
	if (localTime->tm_hour >= 10)
		StringUtils::ReplaceAll(log, "{h}", std::to_string(localTime->tm_hour));
	else
		StringUtils::ReplaceAll(log, "{h}", "0" + std::to_string(localTime->tm_hour));
	//minute
	if (localTime->tm_min >= 10)
		StringUtils::ReplaceAll(log, "{m}", std::to_string(localTime->tm_min));
	else
		StringUtils::ReplaceAll(log, "{m}", "0" + std::to_string(localTime->tm_min));
	//second
	if (localTime->tm_sec >= 10)
		StringUtils::ReplaceAll(log, "{s}", std::to_string(localTime->tm_sec));
	else
		StringUtils::ReplaceAll(log, "{s}", "0" + std::to_string(localTime->tm_sec));
	//millisecond
	if (milliSeconds >= 100)
		StringUtils::ReplaceAll(log, "{ms}", std::to_string(milliSeconds));
	else if (milliSeconds >= 10)
		StringUtils::ReplaceAll(log, "{ms}", "0" + std::to_string(milliSeconds));
	else
		StringUtils::ReplaceAll(log, "{ms}", "00" + std::to_string(milliSeconds));
	std::unique_lock<std::mutex> lock(m_Mutex);
	switch (logType)
	{
	case LogType::Standard:
	{
		StringUtils::ReplaceAll(log, "{log_type}", "");
		std::cout << log << std::endl;
		LogFile(log, "", localTime);
		break;
	}
	case LogType::Warning:
	{
		StringUtils::ReplaceAll(log, "{log_type}", "Warning: ");
		std::cerr << log << std::endl;
		LogFile(log, "-warn", localTime);
		break;
	}
	case LogType::Error:
	{
		StringUtils::ReplaceAll(log, "{log_type}", "Error: ");
		std::cerr << log << std::endl;
		LogFile(log, "-err", localTime);
		break;
	}
	case LogType::Verbose:
	{
		StringUtils::ReplaceAll(log, "{log_type}", "");
		std::cout << log << std::endl;
		break;
	}
	case LogType::Debug:
	{
		StringUtils::ReplaceAll(log, "{log_type}", "Debug: ");
		std::cout << log << std::endl;
		LogFile(log, "-deb", localTime);
		break;
	}
	}
}

void Logger::LogFile(const std::string& log, const std::string& suffix, std::tm* localTime) const
{
	if (m_LogInFile)
	{
		std::string path = m_LogFilePath;
		std::stringstream filename;
		filename << "Nectere-";
		filename << std::to_string(localTime->tm_year + 1900);
		filename << '-';
		if (localTime->tm_mon < 9)
			filename << "0";
		filename << std::to_string(localTime->tm_mon + 1);
		filename << '-';
		if (localTime->tm_mday < 10)
			filename << "0";
		filename << std::to_string(localTime->tm_mday);
		filename << suffix;
		filename << ".log";
		std::filesystem::path logFilePath = std::filesystem::absolute(path + filename.str());
		std::filesystem::create_directories(logFilePath.parent_path());
		std::ofstream outfile;
		outfile.open(logFilePath, std::ios_base::app); // append instead of overwrite
		outfile << log << std::endl;
		outfile.close();
	}
}