#pragma once

#include <string>
#include <vector>

namespace StringUtils
{
	bool Find(const std::string&, char, unsigned long&);
	bool Find(const std::string&, char);
	bool StartWith(const std::string&, const std::string&);
	bool EndWith(const std::string&, const std::string&);
	void ReplaceAll(std::string&, const std::string&, const std::string&);
	void Trim(std::string&);
	size_t Count(const std::string&, const std::string&);
	std::vector<std::string> Split(const std::string&, const std::string&, bool, bool);
	inline std::vector<std::string> Split(const std::string&, const std::string&);
	inline std::vector<std::string> Split(const std::string&, const std::string&, bool);
	int SmartSplit(std::vector<std::string>&, const std::string&, const std::string&, bool);
	int SmartSplit(std::vector<std::string>&, const std::string&, const std::string&);
}