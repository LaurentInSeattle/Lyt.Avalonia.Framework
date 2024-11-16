namespace Lyt.Avalonia.Persistence; 

public sealed record class FileId ( 
    FileManagerModel.Area Area, 
    FileManagerModel.Kind Kind, 
    string Filename );
