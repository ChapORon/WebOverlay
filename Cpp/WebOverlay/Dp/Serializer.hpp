#pragma once

#include <string>
#include <vector>

#define DP_SERIALIZER_DEFINITION(Type) template <> struct Dp::Serializer<Type>\
{\
	Dp::Node ToNode(const Type &value);\
	Type FromNode(const Dp::Node &node);\
}

#define DP_SERIALIZER_DEFINITION_TONODE(Type) template <> struct Dp::Serializer<Type>\
{\
	Dp::Node ToNode(const Type &value);\
	Type FromNode(const Dp::Node &node) = delete;\
}

#define DP_SERIALIZER_DEFINITION_FROMNODE(Type) template <> struct Dp::Serializer<Type>\
{\
	Dp::Node ToNode(const Type &value) = delete;\
	Type FromNode(const Dp::Node &node);\
}

#define DP_SERIALIZER_TONODE(Type) Dp::Node Dp::Serializer<Type>::ToNode(const Type &value)
#define DP_SERIALIZER_FROMNODE(Type) Type Dp::Serializer<Type>::FromNode(const Dp::Node &node)

namespace Dp
{
	class Node;
	template <typename t_TypeToSerialize>
	struct Serializer
	{
		Node ToNode(const t_TypeToSerialize&) = delete;
		t_TypeToSerialize FromNode(const Node&) = delete;
	};
}

DP_SERIALIZER_DEFINITION(bool);
DP_SERIALIZER_DEFINITION(char);
DP_SERIALIZER_DEFINITION(unsigned char);
DP_SERIALIZER_DEFINITION(int);
DP_SERIALIZER_DEFINITION(unsigned int);
DP_SERIALIZER_DEFINITION(float);
DP_SERIALIZER_DEFINITION(double);
DP_SERIALIZER_DEFINITION(long);
DP_SERIALIZER_DEFINITION(unsigned long);
DP_SERIALIZER_DEFINITION(long long);
DP_SERIALIZER_DEFINITION(unsigned long long);
DP_SERIALIZER_DEFINITION(std::string);