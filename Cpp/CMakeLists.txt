cmake_minimum_required (VERSION 3.19)
project (WebOverlay)

set(NAME WebOverlay)

set(Boost_USE_STATIC_LIBS ON)
set(Boost_USE_MULTITHREADED ON)
find_package(Boost 1.76.0 REQUIRED system regex date_time)

set(CMAKE_CXX_STANDARD 20)
set(CMAKE_CXX_VISIBILITY_PRESET hidden)
set(CMAKE_VISIBILITY_INLINES_HIDDEN YES)
set(CMAKE_CXX_FLAGS_DEBUG "${CMAKE_CXX_FLAGS_DEBUG} -DDEBUG=1")

set (SOURCE_DIR ${CMAKE_CURRENT_SOURCE_DIR}/WebOverlay)

include_directories(${SOURCE_DIR})

set(CONCURRENCY_SOURCES
	${SOURCE_DIR}/Concurrency/Task.cpp
	${SOURCE_DIR}/Concurrency/Task.hpp
	${SOURCE_DIR}/Concurrency/TaskResult.hpp
	${SOURCE_DIR}/Concurrency/Thread.cpp
	${SOURCE_DIR}/Concurrency/Thread.hpp
	${SOURCE_DIR}/Concurrency/ThreadSystem.cpp
	${SOURCE_DIR}/Concurrency/ThreadSystem.hpp)

set(DP_SOURCES
	${SOURCE_DIR}/Dp/Format.hpp
	${SOURCE_DIR}/Dp/Json.cpp
	${SOURCE_DIR}/Dp/Json.hpp
	${SOURCE_DIR}/Dp/Node.cpp
	${SOURCE_DIR}/Dp/Node.hpp
	${SOURCE_DIR}/Dp/Result.cpp
	${SOURCE_DIR}/Dp/Result.hpp
	${SOURCE_DIR}/Dp/Serializer.cpp
	${SOURCE_DIR}/Dp/Serializer.hpp
	${SOURCE_DIR}/Dp/StringUtils.cpp
	${SOURCE_DIR}/Dp/StringUtils.hpp
	${SOURCE_DIR}/Dp/Xml.cpp
	${SOURCE_DIR}/Dp/Xml.hpp)

set(NETWORK_SOURCES
	${SOURCE_DIR}/Network/IDGenerator.cpp
	${SOURCE_DIR}/Network/IDGenerator.hpp
	${SOURCE_DIR}/Network/Server.cpp
	${SOURCE_DIR}/Network/Server.hpp
	${SOURCE_DIR}/Network/User.cpp
	${SOURCE_DIR}/Network/User.hpp)

set(SOURCE_FILES
	${CONCURRENCY_SOURCES}
	${DP_SOURCES}
	${NETWORK_SOURCES}
	${SOURCE_DIR}/Logger.cpp
	${SOURCE_DIR}/Logger.hpp
	${SOURCE_DIR}/StringUtils.cpp
	${SOURCE_DIR}/StringUtils.hpp
	${SOURCE_DIR}/main.cpp)

add_executable(${NAME} ${SOURCE_FILES} ${NETWORK_SOURCES})

if (Boost_FOUND)
	add_definitions(-DUSE_BOOST)
	include_directories(${Boost_INCLUDE_DIRS})
	link_directories(${Boost_LIBRARY_DIRS})
	target_link_libraries(${NAME} ${Boost_LIBRARIES})
endif()
