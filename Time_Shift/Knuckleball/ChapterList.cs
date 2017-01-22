// -----------------------------------------------------------------------
// <copyright file="ChapterList.cs" company="Knuckleball Project">
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
//
// Portions created by Jim Evans are Copyright © 2012.
// All Rights Reserved.
//
// Contributors:
//     Jim Evans, james.h.evans.jr@@gmail.com
//
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;

namespace Knuckleball
{
    /// <summary>
    /// Represents the collection of chapters in a file, tracking whether there have
    /// been changes made to the collection.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "ChapterList object has List (ordered) semantics, so a suffix of List is correct.")]
    public sealed class ChapterList : IList<Chapter>
    {
        private List<Chapter> chapters = new List<Chapter>();
        private HashSet<Guid> hashedIndex = new HashSet<Guid>();

        public static event Action<string> OnLog;

        /// <summary>
        /// Prevents a default instance of the <see cref="ChapterList"/> class from being created.
        /// </summary>
        private ChapterList()
        {
        }

        /// <summary>
        /// Gets the number of <see cref="Chapter">Chapters</see> contained in this <see cref="ChapterList"/>.
        /// </summary>
        public int Count => chapters.Count;

        /// <summary>
        /// Gets a value indicating whether this <see cref="ChapterList"/> is read-only.
        /// </summary>
        bool ICollection<Chapter>.IsReadOnly => false;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ChapterList"/> has been modified since being loaded.
        /// </summary>
        private bool IsDirty { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Chapter"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the <see cref="Chapter"/> to get or set.</param>
        /// <returns>The <see cref="Chapter"/> at the specified index.</returns>
        /// <exception cref="ArgumentNullException">value to be set is <see langword="null"/></exception>
        /// <exception cref="ArgumentException">value to be set is already in the list</exception>
        public Chapter this[int index]
        {
            get
            {
                return chapters[index];
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                if (hashedIndex.Contains(value.Id))
                {
                    throw new ArgumentException("Chapter is already in the chapter list", nameof(value));
                }

                chapters[index] = value;
                value.Changed += OnChapterChanged;
                hashedIndex.Add(value.Id);
                IsDirty = true;
            }
        }

        /// <summary>
        /// Adds a <see cref="Chapter"/> to this <see cref="ChapterList"/>.
        /// </summary>
        /// <param name="item">The <see cref="Chapter"/> to add.</param>
        /// <exception cref="ArgumentNullException"><paramref name="item"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="item"/> is already in the list</exception>
        public void Add(Chapter item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (hashedIndex.Contains(item.Id))
            {
                throw new ArgumentException("Chapter is already in the chapter list", nameof(item));
            }

            chapters.Add(item);
            item.Changed += OnChapterChanged;
            hashedIndex.Add(item.Id);
            IsDirty = true;
        }

        /// <summary>
        /// Removes all items from this <see cref="ChapterList"/>
        /// </summary>
        public void Clear()
        {
            if (Count > 0)
            {
                hashedIndex.Clear();
                chapters.Clear();
                IsDirty = true;
            }
        }

        /// <summary>
        /// Determines whether this <see cref="ChapterList"/> contains a specific <see cref="Chapter"/>.
        /// </summary>
        /// <param name="item">The <see cref="Chapter"/> to locate in the <see cref="ChapterList"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="item"/> is found in the <see cref="ChapterList"/>;
        /// otherwise, <see langword="false"/>.</returns>
        public bool Contains(Chapter item)
        {
            return hashedIndex.Contains(item.Id);
        }

        /// <summary>
        /// Copies the <see cref="Chapter">Chapters</see> of this <see cref="ChapterList"/> to an <see cref="Array"/>,
        /// starting at a particular <see cref="Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the
        /// <see cref="Chapter">Chapters</see> copied from this <see cref="ChapterList"/>. The <see cref="Array"/>
        /// must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.</exception>
        /// <exception cref="ArgumentException">The number of elements in this <see cref="ChapterList"/> is greater
        /// than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.</exception>
        public void CopyTo(Chapter[] array, int arrayIndex)
        {
            chapters.CopyTo(array, arrayIndex);
        }

       /// <summary>
        /// Determines the index of a <see cref="Chapter"/> in this <see cref="ChapterList"/>
        /// </summary>
        /// <param name="item">The <see cref="Chapter"/> to locate in the <see cref="ChapterList"/></param>
        /// <returns>The index of <paramref name="item"/> if found in the list; otherwise, -1.</returns>
        public int IndexOf(Chapter item)
        {
            if (!hashedIndex.Contains(item.Id))
            {
                return -1;
            }
            return chapters.IndexOf(item);
        }

        /// <summary>
        /// Inserts an item to this <see cref="ChapterList"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The <see cref="Chapter"/> to insert into the <see cref="ChapterList"/></param>
        /// <exception cref="ArgumentNullException"><paramref name="item"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0,
        /// or <paramref name="index"/> is greater than <see cref="Count"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="item"/> is alrady in the list</exception>
        public void Insert(int index, Chapter item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (hashedIndex.Contains(item.Id))
            {
                throw new ArgumentException("Chapter is already in the chapter list", nameof(item));
            }

            chapters.Insert(index, item);
            item.Changed += OnChapterChanged;
            hashedIndex.Add(item.Id);
            IsDirty = true;
        }

        /// <summary>
        /// Removes the first occurrence of a specific <see cref="Chapter"/> from the <see cref="ChapterList"/>.
        /// </summary>
        /// <param name="item">The <see cref="Chapter"/> to remove from the <see cref="ChapterList"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="item"/> was successfully removed from the
        /// <see cref="ChapterList"/>; otherwise, <see langword="false"/> false. This method also returns
        /// <see langword="false"/> if <paramref name="item"/> is not found in the original<see cref="ChapterList"/>.</returns>
        public bool Remove(Chapter item)
        {
            bool isRemoved = chapters.Remove(item);
            IsDirty = IsDirty || isRemoved;
            if (isRemoved)
            {
                item.Changed -= OnChapterChanged;
                hashedIndex.Remove(item.Id);
            }
            return isRemoved;
        }

        /// <summary>
        /// Removes the <see cref="Chapter"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the <see cref="Chapter"/> to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0,
        /// or <paramref name="index"/> is greater than <see cref="Count"/>.</exception>
        public void RemoveAt(int index)
        {
            Chapter toRemove = this[index];
            hashedIndex.Remove(toRemove.Id);
            toRemove.Changed -= OnChapterChanged;
            chapters.RemoveAt(index);
            IsDirty = true;
        }

        /// <summary>
        /// Returns an enumerator that iterates through a <see cref="ChapterList"/>.
        /// </summary>
        /// <returns>An <see cref="IEnumerator{T}"/> object that can be used to iterate
        /// through the <see cref="ChapterList"/>.</returns>
        public IEnumerator<Chapter> GetEnumerator()
        {
            return chapters.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a <see cref="ChapterList"/>.
        /// </summary>
        /// <returns>An <see cref="System.Collections.IEnumerator"/> object that can be used to iterate
        /// through the <see cref="ChapterList"/>.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return chapters.GetEnumerator();
        }

        /// <summary>
        /// Reads the chapter information from the specified file.
        /// </summary>
        /// <param name="fileHandle">The handle to the file from which to read the chapter information.</param>
        /// <returns>A new instance of a <see cref="ChapterList"/> object containing the information
        /// about the chapters for the file.</returns>
        internal static ChapterList ReadFromFile(IntPtr fileHandle)
        {
            ChapterList list = new ChapterList();
            IntPtr chapterListPointer = IntPtr.Zero;
            int chapterCount = 0;
            NativeMethods.MP4ChapterType chapterType = NativeMethods.MP4GetChapters(fileHandle, ref chapterListPointer, ref chapterCount, NativeMethods.MP4ChapterType.Any);
            OnLog?.Invoke($"Chapter type: {chapterType}");
            if (chapterType != NativeMethods.MP4ChapterType.None && chapterCount != 0)
            {
                IntPtr currentChapterPointer = chapterListPointer;
                for (int i = 0; i < chapterCount; ++i)
                {
                    NativeMethods.MP4Chapter currentChapter = currentChapterPointer.ReadStructure<NativeMethods.MP4Chapter>();
                    TimeSpan duration = TimeSpan.FromMilliseconds(currentChapter.duration);
                    string title = GetString(currentChapter.title);
                    OnLog?.Invoke($"{title} {duration}");
                    list.AddInternal(new Chapter { Duration = duration, Title = title });
                    currentChapterPointer = IntPtr.Add(currentChapterPointer, Marshal.SizeOf(currentChapter));
                }
            }
            else
            {
                /*int timeScale = NativeMethods.MP4GetTimeScale(fileHandle);
                long duration = NativeMethods.MP4GetDuration(fileHandle);
                list.AddInternal(new Chapter { Duration = TimeSpan.FromSeconds(duration / (double)timeScale), Title = "Chapter 1" });*/
                list.AddInternal(new Chapter { Duration = TimeSpan.Zero, Title = "Chapter 1" });
            }
            if (chapterListPointer != IntPtr.Zero)
            {
                NativeMethods.MP4Free(chapterListPointer);
            }
            return list;
        }

        /// <summary>
        /// Decodes a C-Style string into a string, can handle UTF-8 or UTF-16 encoding.
        /// </summary>
        /// <param name="bytes">C-Style string</param>
        /// <returns></returns>
        private static string GetString(byte[] bytes)
        {
            string title = Encoding.UTF8.GetString(bytes);
            if (bytes[0] == 0xFF && bytes[1] == 0xFE)
            {
                title = Encoding.Unicode.GetString(bytes);
            }
            else if (bytes[0] == 0xFE && bytes[1] == 0xFF)
            {
                title = Encoding.BigEndianUnicode.GetString(bytes);
            }
            return title.Substring(0, title.IndexOf('\0'));
        }

        /// <summary>
        /// Adds a <see cref="Chapter"/> to the list without dirtying the list.
        /// </summary>
        /// <param name="toAdd">The <see cref="Chapter"/> to add to the list.</param>
        private void AddInternal(Chapter toAdd)
        {
            chapters.Add(toAdd);
            toAdd.Changed += OnChapterChanged;
            hashedIndex.Add(toAdd.Id);
        }

        private void OnChapterChanged(object sender, EventArgs e)
        {
            IsDirty = true;
        }
    }
}
