[<AutoOpen>]
module Collections

type System.Int32 with
    member n.Times f =
        for i = 0 to n-1 do f i

type System.Collections.Generic.IEnumerable<'a> with
    member lhs.Append rhs = Seq.append lhs rhs
    // 'a -> unit
    member xs.Iter f = Seq.iter f xs   
    // 'a -> int
    member xs.CountBy f = Seq.countBy f xs
    // 'a -> bool
    member xs.ForAll f = Seq.forall f xs
    member xs.Exists f = Seq.exists f xs
    member xs.Filter f = Seq.filter f xs
    member xs.Find f = Seq.find f xs
    // 'a -> 'b
    member xs.Map f = Seq.map f
    member xs.Reduce f = Seq.reduce f xs
    member xs.SortBy f = Seq.sortBy f xs
    member inline xs.SumBy f = Seq.sumBy f xs
    member inline xs.AverageBy f = Seq.averageBy f xs

