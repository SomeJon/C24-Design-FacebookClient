﻿using System;
using FacebookWrapperEnhancements.Code.EnhancedObjects;

namespace FacebookWrapperEnhancements.Code.Collection.Sort.SortingStrategy
{
    public class SortPostsByDatePublished : IPostSortingStrategy
    {
        public Comparison<EnhancedPost> GetComparison()
        {
            return (i_FirstObject, i_SecondObject) => Nullable.Compare(i_SecondObject?.CreatedTime, i_FirstObject?.CreatedTime);
        }

        public override string ToString()
        {
            return "Sort by Date Published";
        }

        public override bool Equals(object i_Obj)
        {
            return i_Obj != null && this.GetType() == i_Obj.GetType();
        }

        public override int GetHashCode()
        {
            return this.GetType().GetHashCode();
        }
    }
}