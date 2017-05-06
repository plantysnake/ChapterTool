using System.Collections;
using System.Collections.Generic;

namespace ChapterTool.Util
{
    public class ChapterInfoGroup : List<ChapterInfo>
    {
    }
    public class BDMVGroup        : ChapterInfoGroup  { }
    public class IfoGroup         : ChapterInfoGroup  { }
    public class XplGroup         : ChapterInfoGroup  { }
    public class MplsGroup        : ChapterInfoGroup  { }
    public class XmlGroup         : ChapterInfoGroup  { }
}
