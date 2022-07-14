#pragma once

namespace Concurrency
{
    enum class TaskResult : int
    {
        Success,
        Fail,
        NeedUpdate
    };
}