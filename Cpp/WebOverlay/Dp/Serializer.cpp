#include "Serializer.hpp"

#include "Node.hpp"

DP_SERIALIZER_TONODE(bool) { return Node("", value ? "true" : "false"); }
DP_SERIALIZER_FROMNODE(bool) { return node.GetValue() == "true"; }

DP_SERIALIZER_TONODE(char) { return Node("", std::to_string(value)); }
DP_SERIALIZER_FROMNODE(char) { return node.GetValue()[0]; }

DP_SERIALIZER_TONODE(unsigned char) { return Node("", std::to_string(value)); }
DP_SERIALIZER_FROMNODE(unsigned char) { return static_cast<unsigned char>(node.GetValue()[0]); }

DP_SERIALIZER_TONODE(int) { return Node("", std::to_string(value)); }
DP_SERIALIZER_FROMNODE(int) { return std::stoi(node.GetValue()); }

DP_SERIALIZER_TONODE(unsigned int) { return Node("", std::to_string(value)); }
DP_SERIALIZER_FROMNODE(unsigned int) { return static_cast<unsigned int>(std::stol(node.GetValue())); }

DP_SERIALIZER_TONODE(float) { return Node("", std::to_string(value)); }
DP_SERIALIZER_FROMNODE(float) { return std::stof(node.GetValue()); }

DP_SERIALIZER_TONODE(double) { return Node("", std::to_string(value)); }
DP_SERIALIZER_FROMNODE(double) { return std::stod(node.GetValue()); }

DP_SERIALIZER_TONODE(long) { return Node("", std::to_string(value)); }
DP_SERIALIZER_FROMNODE(long) { return std::stol(node.GetValue()); }

DP_SERIALIZER_TONODE(unsigned long) { return Node("", std::to_string(value)); }
DP_SERIALIZER_FROMNODE(unsigned long) { return std::stoul(node.GetValue()); }

DP_SERIALIZER_TONODE(long long) { return Node("", std::to_string(value)); }
DP_SERIALIZER_FROMNODE(long long) { return std::stoll(node.GetValue()); }

DP_SERIALIZER_TONODE(unsigned long long) { return Node("", std::to_string(value)); }
DP_SERIALIZER_FROMNODE(unsigned long long) { return std::stoull(node.GetValue()); }

DP_SERIALIZER_TONODE(std::string) { return Node("", value); }
DP_SERIALIZER_FROMNODE(std::string) { return node.GetValue(); }