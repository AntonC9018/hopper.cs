using Hopper.Utils.FS;
using Hopper.Utils;
using System;

namespace Hopper.Core.Stats
{
    public interface IModifier
    {
        void AddSelf(StatManager sm);
        void RemoveSelf(StatManager sm);
    }

    public class Modifier
    {
        static IdGenerator s_idGenerator = new IdGenerator();
        public readonly int id = s_idGenerator.GetNextId();

        public override int GetHashCode()
        {
            return id;
        }

        public static StatModifier<T> Create<T>(SimpleStatPath<T> path, T file)
            where T : StatFile, IAddableWith<T>, new()
        {
            return new StatModifier<T>(path, file);
        }

        public static ChainModifier<T> Create<T>(SimpleStatPath<T> path, Action<StatEvent<T>> handler)
            where T : StatFile, IAddableWith<T>, new()
        {
            return new ChainModifier<T>(path, handler);
        }
    }

    public class StatModifier<T> : Modifier, IModifier where T : File, IAddableWith<T>
    {
        public T file;
        public IStatPath<T> path;

        public StatModifier(IStatPath<T> path, T file)
        {
            this.file = file;
            this.path = path;
        }

        public void AddSelf(StatManager sm)
        {
            sm.AddStatModifier<T>(this);
        }

        public void RemoveSelf(StatManager sm)
        {
            sm.RemoveStatModifier<T>(this);
        }
    }

    public class ChainModifier<T> : Modifier, IModifier where T : File
    {
        public Action<StatEvent<T>> handler;
        public IStatPath<T> path;

        public ChainModifier(IStatPath<T> path, Action<StatEvent<T>> handler)
        {
            this.handler = handler;
            this.path = path;
        }

        public void AddSelf(StatManager sm)
        {
            sm.AddChainModifier<T>(this);
        }

        public void RemoveSelf(StatManager sm)
        {
            sm.RemoveChainModifier<T>(this);
        }
    }
}