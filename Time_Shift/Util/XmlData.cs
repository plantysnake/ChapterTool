using System;
using System.Linq;
using System.Xml;
using System.Collections.Generic;
using static ChapterTool.Util.ConvertMethod;

namespace ChapterTool.Util
{
    public static class XmlData
    {
        public static IEnumerable<ChapterInfo> PraseXml(XmlDocument doc)
        {
            XmlElement root = doc.DocumentElement;
            if (root == null)
            {
                throw new ArgumentException("Empty Xml file");
            }
            if (root.Name != "Chapters")
            {
                throw new Exception($"Invalid Xml file.\nroot node Name: {root.Name}");
            }

            foreach (XmlNode editionEntry in root.ChildNodes)//Get Entrance for each chapter
            {
                if (editionEntry.NodeType == XmlNodeType.Comment) continue;
                if (editionEntry.Name != "EditionEntry")
                {
                    throw new Exception($"Invalid Xml file.\nEntry Name: {editionEntry.Name}");
                }
                ChapterInfo buff = new ChapterInfo { SourceType = "XML", Tag = doc, TagType = doc.GetType() };
                int index = 0;
                foreach (XmlNode editionEntryChildNode in ((XmlElement)editionEntry).ChildNodes)//Get all the child nodes in current chapter
                {
                    if (editionEntryChildNode.Name != "ChapterAtom") continue;
                    buff.Chapters.AddRange(PraseChapterAtom(editionEntryChildNode, ++index));
                }

                for (int i = 0; i < buff.Chapters.Count - 1; i++)
                {
                    if (buff.Chapters[i].Time == buff.Chapters[i + 1].Time)
                    {
                        buff.Chapters.Remove(buff.Chapters[i--]);
                    }
                }
                //buff.Chapters = buff.Chapters.Distinct().ToList();
                yield return buff;
            }
        }

        private static IEnumerable<Chapter> PraseChapterAtom(XmlNode chapterAtom, int index)
        {
            Chapter startChapter = new Chapter { Number = index };
            Chapter endChapter = new Chapter { Number = index };
            var innerChapterAtom = new List<Chapter>();
            foreach (XmlNode chapterAtomChildNode in ((XmlElement)chapterAtom).ChildNodes) //Get detail info for current chapter node
            {
                switch (chapterAtomChildNode.Name)
                {
                    case "ChapterTimeStart":
                        startChapter.Time = RTimeFormat.Match(chapterAtomChildNode.InnerText).Value.ToTimeSpan();
                        break;
                    case "ChapterTimeEnd":
                        endChapter.Time = RTimeFormat.Match(chapterAtomChildNode.InnerText).Value.ToTimeSpan();
                        break;
                    case "ChapterDisplay":
                        try
                        {
                            startChapter.Name = ((XmlElement)chapterAtomChildNode).ChildNodes.Cast<XmlNode>().First(node => node.Name == "ChapterString").InnerText;
                        }
                        catch
                        {
                            startChapter.Name = string.Empty;
                        }
                        endChapter.Name = startChapter.Name;
                        break;
                    case "ChapterAtom"://Handling sub chapters.
                        innerChapterAtom.AddRange(PraseChapterAtom(chapterAtomChildNode, index));
                        break;
                }
            }
            yield return startChapter;

            foreach (var chapter in innerChapterAtom)
            {
                yield return chapter;
            }

            if (endChapter.Time.TotalSeconds > startChapter.Time.TotalSeconds)
            {
                yield return endChapter;
            }
        }
    }
}