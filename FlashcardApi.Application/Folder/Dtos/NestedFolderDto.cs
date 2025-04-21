using System;
using System.Collections.Generic;
using FlashcardApi.Application.Desk.Dtos;

namespace FlashcardApi.Application.Folder.Dtos;

public class NestedFolderDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string? ParentFolderId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastModified { get; set; }
    public List<NestedFolderDto> SubFolders { get; set; } = new List<NestedFolderDto>();
    public List<DeskDto> Desks { get; set; } = new List<DeskDto>();
}