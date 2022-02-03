﻿// Skeleton implementation written by Joe Zachary for CS 3500, September 2013.
// Version 1.1 (Fixed error in comment for RemoveDependency.)
// Version 1.2 - Daniel Kopta 
//               (Clarified meaning of dependent and dependee.)
//               (Clarified names in solution/project structure.)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpreadsheetUtilities
{

    /// <summary>
    ///  /// <summary>
    /// Author: Joe Zachary, Daniel Kopta, H. James de St. Germain & Abhiveer Sharma
    /// Partner: None
    /// Date of Creation: January 28, 2022
    /// Course: CS 3500, University of Utah, School of Computing
    /// Copyright: CS 3500 and Abhiveer Sharma - This work may not be copied for use in Academic Coursework. 
    /// I, Abhiveer Sharma, certify that I wrote this code from scratch and did not copy it in part or whole from  
    /// another source. All references used in the completion of the assignment are cited in my README file. 
    /// </summary>
    /// (s1,t1) is an ordered pair of strings
    /// t1 depends on s1; s1 must be evaluated before t1
    /// 
    /// A DependencyGraph can be modeled as a set of ordered pairs of strings.  Two ordered pairs
    /// (s1,t1) and (s2,t2) are considered equal if and only if s1 equals s2 and t1 equals t2.
    /// Recall that sets never contain duplicates.  If an attempt is made to add an element to a 
    /// set, and the element is already in the set, the set remains unchanged.
    /// 
    /// Given a DependencyGraph DG:
    /// 
    ///    (1) If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
    ///        (The set of things that depend on s)    
    ///        
    ///    (2) If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
    ///        (The set of things that s depends on) 
    //
    // For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    //     dependents("a") = {"b", "c"}
    //     dependents("b") = {"d"}
    //     dependents("c") = {}
    //     dependents("d") = {"d"}
    //     dependees("a") = {}
    //     dependees("b") = {"a"}
    //     dependees("c") = {"a"}
    //     dependees("d") = {"b", "d"}
    /// </summary>
    public class DependencyGraph
    {
        private Dictionary<string, HashSet<string>> dependents;
        private Dictionary<string, HashSet<string>> dependees;


        /// <summary>
        /// Creates an empty DependencyGraph.
        /// </summary>
        public DependencyGraph()
        {
            dependees = new Dictionary<string, HashSet<string>>();
            dependents = new Dictionary<string, HashSet<string>>();
            this.Size = 0;

        }


        /// <summary>
        /// The number of ordered pairs in the DependencyGraph.
        /// </summary>
        public int Size
        {
            private set;
            get;
        }


        /// <summary>
        /// The size of dependees(s).
        /// This property is an example of an indexer.  If dg is a DependencyGraph, you would
        /// invoke it like this:
        /// dg["a"]
        /// It should return the size of dependees("a")
        /// </summary>
        public int this[string s]
        {

            get
            {
                if (HasDependees(s))
                    return dependees[s].Count;
                return 0;
            }
        }


        /// <summary>
        /// Reports whether dependents(s) is non-empty.
        /// </summary>
        public bool HasDependents(string s)
        {
            if (dependents.ContainsKey(s))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// Reports whether dependees(s) is non-empty.
        /// </summary>
        public bool HasDependees(string s)
        {
            if (dependees.ContainsKey(s))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// Enumerates dependents(s).
        /// </summary>
        public IEnumerable<string> GetDependents(string s)
        {
            HashSet<string> _dependents = new HashSet<string>();
            // List<string> _dependents1 = new List<string>();
            //if(dependents.TryGetValue(s, out _dependents))
            //{
            //    _dependents.CopyTo(_dependents1);
            // }
            if (dependents.ContainsKey(s))
            {
                foreach (string _val in dependents[s])
                {
                    _dependents.Add(_val);
                }
            }

            return _dependents;

        }

        /// <summary>
        /// Enumerates dependees(s).
        /// </summary>
        public IEnumerable<string> GetDependees(string s)
        {
            HashSet<string> _dependees = new HashSet<string>();
            if (dependees.ContainsKey(s))
            {
                foreach (string _val in dependees[s])
                {
                    _dependees.Add(_val);
                }
            }

            return _dependees;
        }


        /// <summary>
        /// <para>Adds the ordered pair (s,t), if it doesn't exist</para>
        /// 
        /// <para>This should be thought of as:</para>   
        /// 
        ///   t depends on s
        ///
        /// </summary>
        /// <param name="s"> s must be evaluated first. T depends on S</param>
        /// <param name="t"> t cannot be evaluated until s is</param>        /// 
        public void AddDependency(string s, string t)
        {
            if (dependents.ContainsKey(s))
            {
                if (!dependents[s].Contains(t))
                {
                    dependents[s].Add(t);
                    Size++;
                }

            }
            else
            {
                HashSet<string> _dependents = new HashSet<string>();
                _dependents.Add(t);
                dependents.Add(s, _dependents);
                Size++;
            }

            if (dependees.ContainsKey(t))
            {
                dependees[t].Add(s);
            }
            else
            {
                HashSet<string> _dependees = new HashSet<string>();
                _dependees.Add(s);
                dependees.Add(t, _dependees);

            }

        }


        /// <summary>
        /// Removes the ordered pair (s,t), if it exists
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        public void RemoveDependency(string s, string t)
        {
            if (dependents.ContainsKey(s))
            {
                dependents[s].Remove(t);
                Size--;

            }

            if (dependees.ContainsKey(t))
            {
                dependees[t].Remove(s);

            }

        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (s,r).  Then, for each
        /// t in newDependents, adds the ordered pair (s,t).
        /// </summary>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
            HashSet<string> oldDependents = new HashSet<string>(GetDependents(s));
            foreach (string oldDependent in oldDependents)
            {
                RemoveDependency(s, oldDependent);
            }
            foreach (string newDependent in newDependents)
            {
                AddDependency(s, newDependent);
            }

        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (r,s).  Then, for each 
        /// t in newDependees, adds the ordered pair (t,s).
        /// </summary>
        public void ReplaceDependees(string s, IEnumerable<string> newDependees)
        {
            HashSet<string> oldDependees = new HashSet<string>(GetDependees(s));
            foreach (string oldDependee in oldDependees)
            {
                RemoveDependency(oldDependee, s);
            }
            foreach (string newDependee in newDependees)
            {
                AddDependency(newDependee, s);
            }

        }

    }

}