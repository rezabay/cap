﻿using FlexBus.Messages;

namespace FlexBus.Monitoring;

public class MessageQueryDto
{
    public MessageType MessageType { get; set; }

    public string Group { get; set; }

    public string Name { get; set; }

    public string Content { get; set; }

    public string StatusName { get; set; }

    public int CurrentPage { get; set; }

    public int PageSize { get; set; }
}