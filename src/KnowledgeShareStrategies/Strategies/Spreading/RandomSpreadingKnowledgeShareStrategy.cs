﻿using System;
using System.Linq;

namespace RelationalGit
{
    public class RandomSpreadingKnowledgeShareStrategy : KnowledgeShareStrategy
    {
        private Random _random = new Random();

        internal override string[] RecommendReviewers(PullRequestContext pullRequestContext)
        {
            var availableDevelopers = pullRequestContext.AvailableDevelopers.Where(q => q.TotalCommits > 10 || q.TotalReviews > 10).Select(q => q.NormalizedName).ToArray();

            pullRequestContext.PRKnowledgeables = pullRequestContext.PRKnowledgeables
                .OrderBy(q => q.NumberOfReviews)
                .OrderBy(q => q.NumberOfReviewedFiles)
                .OrderBy(q => q.NumberOfCommits)
                .ToArray();

            var experiencedDevelopers = pullRequestContext.PRKnowledgeables.Select(q => q.DeveloperName);
            var nonexperiencedDevelopers = availableDevelopers.Except(experiencedDevelopers).ToArray();

            if (nonexperiencedDevelopers.Length == 0)
                return pullRequestContext.ActualReviewers;

            var randomDeveloper = nonexperiencedDevelopers[_random.Next(0, nonexperiencedDevelopers.Length)];

            var leastKnowledgeable = pullRequestContext.WhoHasTheLeastKnowledge();

            var leastIndex = Array.FindIndex(pullRequestContext.ActualReviewers,q=>q== leastKnowledgeable);

            pullRequestContext.ActualReviewers[leastIndex] = randomDeveloper;

            return pullRequestContext.ActualReviewers;
        }
        
    }
}