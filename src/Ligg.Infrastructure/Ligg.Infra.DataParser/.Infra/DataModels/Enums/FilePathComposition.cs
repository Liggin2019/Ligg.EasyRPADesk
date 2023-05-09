namespace Ligg.Infrastructure.DataModels
{
    internal enum FilePathComposition
    {
        FilePathTitle = 0,//e.g. d:\tmp\this-is-a-photo
        Directory = 1,//e.g. d:\tmp
        Folder = 2,//e.g. tmp
        FileName = 10,//e.g. this-is-a-photo.jpg
        FileTitle = 11,//e.g. this-is-a-photo
        Postfix = 12,//e.g. .jpg
        Suffix = 13//e.g. jpg
    }
}
