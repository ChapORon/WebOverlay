#pragma once

#include "Dp/Format.hpp"
#include "Dp/Node.hpp"
#include "Dp/Result.hpp"

namespace Dp
{
    namespace Json
    {
        std::string Str(const Node&, const Format&);
        std::string Str(const Node&, unsigned int = 2, unsigned int = 0, bool = true);
        void Write(const Node&, std::ostream&, unsigned int = 2);
        void Write(const Node&, const std::string&, unsigned int = 2);
        Node LoadFromFile(const std::string&);
        Node LoadFromStream(std::istream&);
        Node LoadFromContent(const std::string&);
    };
}
