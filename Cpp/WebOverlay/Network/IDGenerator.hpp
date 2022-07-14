#pragma once

#include <mutex>
#include <queue>

namespace Network
{
	class IDGenerator
	{
	private:
		uint16_t m_ID;
		std::queue<uint16_t> m_FreedID;
		std::mutex m_Mutex;

	public:
		IDGenerator();
		IDGenerator(uint16_t);
		uint16_t GenerateID();
		void FreeID(uint16_t);
	};
}