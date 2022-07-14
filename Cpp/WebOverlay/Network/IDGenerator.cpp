#include "IDGenerator.hpp"

namespace Network
{
	IDGenerator::IDGenerator() : m_ID(0) {}
	IDGenerator::IDGenerator(uint16_t firstID) : m_ID(firstID) {}

	uint16_t IDGenerator::GenerateID()
	{
		uint16_t id;
		m_Mutex.lock();
		if (m_FreedID.empty())
		{
			id = m_ID;
			++m_ID;
		}
		else
		{
			id = m_FreedID.front();
			m_FreedID.pop();
		}
		m_Mutex.unlock();
		return id;
	}

	void IDGenerator::FreeID(uint16_t id)
	{
		m_Mutex.lock();
		if (m_ID == (id + 1))
			--m_ID;
		else
			m_FreedID.push(id);
		m_Mutex.unlock();
	}
}