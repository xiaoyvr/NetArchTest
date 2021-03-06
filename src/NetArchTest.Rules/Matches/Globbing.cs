using System;
using System.Text.RegularExpressions;
using static NetArchTest.Rules.Matches.MatchFuncs;

namespace NetArchTest.Rules.Matches
{
    public class Globbing
    {
        private readonly Func<string, bool> _match;

        private Globbing(Func<string, bool> match)
        {
            this._match = match;
        }
        
        public static implicit operator Func<string, bool>(Globbing matchSet)
        {
            return matchSet._match;
        }
        
        public static Globbing Empty() => new Globbing(target => false);
        
        public static implicit operator Globbing(string pattern)
        {
            return New(pattern);
        }
        
        public static Globbing operator +(Globbing left, Globbing right)
        {
            return new Globbing(Include<string>(left, right));
        }
                
        public static Globbing operator -(Globbing left, Globbing right)
        {
            return new Globbing(Exclude<string>(left, right));
        }


        public static Globbing New(string pattern)
        {
            return new Globbing(name =>
            {
                var regexPattern = $"^{Regex.Escape(pattern)}$"
                    .Replace(@"\*", ".*")
                    .Replace(@"\?",".");

                if (new Regex(regexPattern).Match(name).Success)
                {
                    return true;
                }

                if (pattern.EndsWith(".*"))
                {
                    var newPattern = $"^{Regex.Escape(pattern.Substring(0, pattern.Length - 2))}$"
                        .Replace(@"\*", ".*")
                        .Replace(@"\?", ".");
                    return new Regex(newPattern).Match(name).Success;
                }
                return false;
            });
        }
    }
}