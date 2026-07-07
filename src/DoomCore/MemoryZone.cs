using System;
using System.Collections.Generic;
using System.Linq;

namespace DoomCore
{
    // Managed simulation of DOOM's zone memory allocator (z_zone.c)
    public class MemoryZone
    {
        private const int ZONEID = 0x1d4a11;
        private const int MINFRAGMENT = 64;
        private const int HeaderSize = 16; // simulated header size for memblock_t

        private readonly MemBlock _blocklist; // sentinel
        public MemBlock? Rover { get; private set; }
        public int Size { get; }

        public MemoryZone(int totalSize)
        {
            if (totalSize <= HeaderSize * 2)
                throw new ArgumentException("zone too small");

            Size = totalSize;
            _blocklist = new MemBlock() { Size = 0, User = this, Tag = 0, Id = 0 };
            ClearZone();
        }

        public void ClearZone()
        {
            var block = new MemBlock
            {
                Size = Size - HeaderSize,
                User = null,
                Tag = 0,
                Id = 0
            };

            _blocklist.Next = block;
            _blocklist.Prev = block;
            _blocklist.User = this;
            _blocklist.Tag = 1; // PU_STATIC

            block.Prev = _blocklist;
            block.Next = _blocklist;

            Rover = block;
        }

        public object Allocate(int requestSize, int tag, object? owner)
        {
            int size = (requestSize + 3) & ~3;
            size += HeaderSize;

            var baseBlock = Rover.Prev.User == null ? Rover.Prev : Rover; // mimic back up over free
            var start = baseBlock.Prev;
            var rover = baseBlock;

            while (true)
            {
                if (rover == start && (rover.User != null || rover.Size < size))
                {
                    // scanned all the way
                    throw new OutOfMemoryException($"Z_Malloc: failed allocation of {size} bytes");
                }

                    if (rover.User != null)
                    {
                    if (rover.Tag < 100) // PU_PURGELEVEL
                    {
                            baseBlock = rover.Next!;
                        rover = baseBlock;
                    }
                    else
                    {
                            baseBlock = baseBlock.Prev!;
                            // free the rover block (by resolving its owner)
                            if (rover.User is BlockOwner ownerRef)
                                Free(ownerRef);
                            baseBlock = baseBlock.Next!;
                            rover = baseBlock.Next!;
                        }
                    }
                    else
                    {
                        rover = rover.Next!;
                    }

                    if (baseBlock.User == null && baseBlock.Size >= size)
                        break;
                }

            var extra = baseBlock.Size - size;
            if (extra > MINFRAGMENT)
            {
                var newblock = new MemBlock
                {
                    Size = extra,
                    User = null,
                    Tag = 0,
                    Prev = baseBlock,
                    Next = baseBlock.Next
                };

                newblock.Next.Prev = newblock;
                baseBlock.Next = newblock;
                baseBlock.Size = size;
            }

            if (owner != null)
            {
                var bo = new BlockOwner { Owner = owner };
                bo.Ref = baseBlock;
                baseBlock.User = bo;
            }
            else
            {
                if (tag >= 100)
                    throw new InvalidOperationException("owner required for purgable blocks");
                baseBlock.User = new BlockOwner { Owner = (object)2, Ref = baseBlock }; // marker
            }

            baseBlock.Tag = tag;
            baseBlock.Id = ZONEID;
            Rover = baseBlock.Next;

            // return a small token object that represents the pointer (in real C code it's pointer into memory)
            return baseBlock.User;
        }

        public void Free(object ptr)
        {
            if (ptr is BlockOwner bo && bo.Ref is MemBlock block)
            {
                if (block.Id != ZONEID)
                    throw new InvalidOperationException("Z_Free: freed a pointer without ZONEID");

                // clear user
                block.User = null;
                block.Tag = 0;
                block.Id = 0;

                var other = block.Prev!;
                if (other.User == null)
                {
                    other.Size += block.Size;
                    other.Next = block.Next;
                    other.Next!.Prev = other;

                    if (block == Rover)
                        Rover = other;

                    block = other;
                }

                other = block.Next!;
                if (other.User == null)
                {
                    block.Size += other.Size;
                    block.Next = other.Next;
                    block.Next!.Prev = block;

                    if (other == Rover)
                        Rover = block;
                }
            }
            else
            {
                // ignore for now - non-block pointer
            }
        }

        public void FreeTags(int lowtag, int hightag)
        {
            var block = _blocklist.Next;
            while (block != _blocklist)
            {
                var next = block.Next;
                if (block.User != null && block.Tag >= lowtag && block.Tag <= hightag)
                {
                    Free(block.User);
                }
                block = next;
            }
        }

        public int FreeMemory()
        {
            int free = 0;
            var block = _blocklist.Next;
            while (block != _blocklist)
            {
                if (block.User == null || block.Tag >= 100)
                    free += block.Size;
                block = block.Next;
            }
            return free;
        }

        // For tests and inspection
        public IEnumerable<MemBlock> Blocks()
        {
            var block = _blocklist.Next;
            while (block != _blocklist)
            {
                yield return block;
                block = block.Next;
            }
        }

        public class MemBlock
        {
            public int Size;
            public object? User;
            public int Tag;
            public int Id;
            public MemBlock? Next;
            public MemBlock? Prev;
        }

        public class BlockOwner
        {
            public object? Owner;
            public MemBlock? Ref;
        }
    }
}
