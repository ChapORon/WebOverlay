#pragma once

#include "Dp/Format.hpp"
#include "Dp/Node.hpp"
#include "Dp/Result.hpp"
#include "Dp/StringUtils.hpp"

namespace Dp
{
    namespace Xml
    {
        Node Create(const std::string&, const std::string & = "1.0", const std::string & = "UTF-8");
        template <typename t_AttributeType>
        void AddAttribute(Node&, const std::string&, const t_AttributeType&);
        void AddDeclaration(Node&, const std::string&, const std::string&, const std::string&);
        std::string Str(const Node&, const Format&);
        std::string Str(const Node&, unsigned int = 2, unsigned int = 0, bool = true);
        void Write(const Node&, std::ostream&, unsigned int = 2);
        void Write(const Node&, const std::string&, unsigned int = 2);
        Node LoadFromFile(const std::string&);
        Node LoadFromStream(std::istream&);
        Node LoadFromContent(const std::string&);
    };
}
