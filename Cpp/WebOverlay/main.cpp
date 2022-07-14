#include "Concurrency/ThreadSystem.hpp"
#include "Network/Server.hpp"

int main(int argc, char **argv)
{
	if (std::shared_ptr<Logger> logger = std::make_shared<Logger>("[{d}-{M}-{y} {h}:{m}:{s}.{ms}] {log_type}{msg}", true))
	{
		if (std::shared_ptr<Concurrency::ThreadSystem> threadSystem = std::make_shared<Concurrency::ThreadSystem>(logger))
		{
			threadSystem->Start();
			if (std::shared_ptr<Network::Server> server = std::make_shared<Network::Server>(4242, "", threadSystem, logger))
			{
				threadSystem->SetOnStopCallback([=]() { server->Stop(); });
				if (server->Start())
					threadSystem->Await();
				return 0;
			}
		}
	}
	return 1;
}
