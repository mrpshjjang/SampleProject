using System.Collections.Generic;
using System.Reflection;

namespace Sample.SpecData
{
    // 시트 데이타
    [Obfuscation(Exclude = false, ApplyToMembers = true)]
    public interface ISheetData
    {
    }

    // 시트 데이타 Dictionary 타입
    [Obfuscation(Exclude = false, ApplyToMembers = true)]
    public interface ISpecData<in K, out T> where T : class, ISheetData
    {
        [JetBrains.Annotations.ItemNotNull]
        IReadOnlyList<T> All { get; }

        // 자동 생성 코드 InnerDataGroupByFieldName 에서 리플렉션 참조 사용 (난독화 제외)
        [Obfuscation(Exclude = true)]
        [JetBrains.Annotations.CanBeNull]
        T Get(K id);

        [JetBrains.Annotations.CanBeNull]
        T this[K id] { get; }
    }
}
