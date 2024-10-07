﻿using System;
using System.Collections.Generic;
using System.Linq;
using FacebookWrapper.ObjectModel;
using FacebookWrapperEnhancements.Code.Collection.Sort;
using FacebookWrapperEnhancements.Code.EnhancedObjects;

namespace FacebookWrapperEnhancements.Code.Collection.Filter
{
    public class FilterData
    {
        public EnhancedUser UserSource { get; set; }
        public List<User> AvailableUsersToSelect { get; set; } = new List<User>();
        public Dictionary<FilterMethod.eFilterCondition, bool> Conditions { get; set; } =
            Enum.GetValues(typeof(FilterMethod.eFilterCondition))
                .Cast<FilterMethod.eFilterCondition>()
                .ToDictionary(i_Condition => i_Condition, i_Condition => false);
        public DateTime MinDate { get; set; } =
            new System.DateTime
                (1900, 1, 1, 0, 0, 0, 0);
        public DateTime MaxDate { get; set; } = DateTime.Now;
        public SortingMethodFactory.eSortingMethod PostSortingMethod { get; set; } =
            SortingMethodFactory.eSortingMethod.ByDatePublished;
        public bool ReverseOrder { get; set; } = false;
        public bool MatchAllFilters { get; set; } = false;
        public string TextContainsString { get; set; } = null;

        public static long ToUnixTimestamp(DateTime i_DateTime)
        {
            DateTimeOffset dateTimeOffset = new DateTimeOffset(i_DateTime.ToUniversalTime());
            return dateTimeOffset.ToUnixTimeSeconds();
        }

        public FilterData DeepClone()
        {
            FilterData clone = new FilterData
                                   {
                                       UserSource = this.UserSource,
                                       AvailableUsersToSelect = new List<User>(this.AvailableUsersToSelect),
                                       Conditions = new Dictionary<FilterMethod.eFilterCondition, bool>(this.Conditions),
                                       MinDate = this.MinDate,
                                       MaxDate = this.MaxDate,
                                       PostSortingMethod = this.PostSortingMethod,
                                       ReverseOrder = this.ReverseOrder,
                                       MatchAllFilters = this.MatchAllFilters,
                                       TextContainsString = this.TextContainsString
                                   };

            return clone;
        }

        // Method to generate a Predicate<EnhancedPost> based on filter conditions
        public Predicate<EnhancedPost> GetPostFilterStrategy()
        {
            FilterMethod.MatchAllFilters = MatchAllFilters;
            return FilterMethod.GetCombinedFilter(Conditions, TextContainsString);
        }

        // Method to generate a Comparison<EnhancedPost> based on sorting method and reverse order flag
        public Comparison<EnhancedPost> GetPostSortStrategy()
        {
            Comparison<EnhancedPost> baseComparison = SortingMethodFactory.GetComparison(PostSortingMethod);

            if (ReverseOrder)
            {
                // If ReverseOrder is true, reverse the comparison
                return (firstPost, secondPost) => baseComparison(secondPost, firstPost);
            }

            return baseComparison;
        }
    }
}
