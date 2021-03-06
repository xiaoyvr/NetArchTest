﻿using NetArchTest.Rules.Matches;
using static NetArchTest.Rules.Matchers;

namespace NetArchTest.Rules.UnitTests
{
    using System;
    using System.Linq;
    using System.Reflection;
    using NetArchTest.TestStructure.Abstract;
    using NetArchTest.TestStructure.Classes;
    using NetArchTest.TestStructure.CustomAttributes;
    using NetArchTest.TestStructure.Dependencies;
    using NetArchTest.TestStructure.Dependencies.Implementation;
    using NetArchTest.TestStructure.Generic;
    using NetArchTest.TestStructure.Inheritance;
    using NetArchTest.TestStructure.Interfaces;
    using NetArchTest.TestStructure.NameMatching.Namespace1;
    using NetArchTest.TestStructure.NameMatching.Namespace2;
    using NetArchTest.TestStructure.NameMatching.Namespace2.Namespace3;
    using NetArchTest.TestStructure.Nested;
    using NetArchTest.TestStructure.Scope;
    using NetArchTest.TestStructure.Sealed;
    using Xunit;

    public class PredicateTests
    {
        [Fact(DisplayName = "Types can be selected by name name.")]
        public void HaveName_MatchFound_ClassesSelected()
        {
            var result = Types
                .InAssembly(Assembly.GetAssembly(typeof(ClassA1)))
                .That(ResideInNamespace("NetArchTest.TestStructure.NameMatching") & HaveName("ClassA1"))
                .GetTypes();

            Assert.Single(result); // Only one type found
            Assert.Equal<Type>(typeof(ClassA1), result.First()); // The correct type found
        }

        [Fact(DisplayName = "Types can be selected if they do not have a specific name.")]
        public void DoNotHaveName_MatchFound_ClassesSelected()
        {
            var result = Types
                .InAssembly(Assembly.GetAssembly(typeof(ClassA1)))
                .That(ResideInNamespace("NetArchTest.TestStructure.NameMatching") & !HaveName("ClassA1"))
                .GetTypes();

            Assert.Equal(4, result.Count()); // three types found
            Assert.Contains<Type>(typeof(ClassA2), result);
            Assert.Contains<Type>(typeof(ClassB1), result);
            Assert.Contains<Type>(typeof(ClassB2), result);
            Assert.Contains<Type>(typeof(ClassA3), result);
        }

        [Fact(DisplayName = "Types can be selected by the start of their name.")]
        public void HaveNameStarting_MatchesFound_ClassesSelected()
        {
            var result = Types
                .InAssembly(Assembly.GetAssembly(typeof(ClassA1)))
                .That(ResideInNamespace("NetArchTest.TestStructure.NameMatching")
                & HaveNameStartingWith("ClassA"))
                .GetTypes();

            Assert.Equal(3, result.Count()); // Three types found
            Assert.Contains<Type>(typeof(ClassA1), result);
            Assert.Contains<Type>(typeof(ClassA2), result);
            Assert.Contains<Type>(typeof(ClassA3), result);
        }

        [Fact(DisplayName = "Types can be selected if their name does not have a specific start.")]
        public void DoNotHaveNameStarting_MatchesFound_ClassesSelected()
        {
            var result = Types
                .InAssembly(Assembly.GetAssembly(typeof(ClassA1)))
                .That(ResideInNamespace("NetArchTest.TestStructure.NameMatching")
                    & !HaveNameStartingWith("ClassA"))
                .GetTypes();

            Assert.Equal(2, result.Count()); // Two types found
            Assert.Contains<Type>(typeof(ClassB1), result);
            Assert.Contains<Type>(typeof(ClassB2), result);
        }

        [Fact(DisplayName = "Types can be selected by the end of their name.")]
        public void HaveNameEnding_MatchesFound_ClassesSelected()
        {
            var result = Types
                .InAssembly(Assembly.GetAssembly(typeof(ClassA1)))
                .That(ResideInNamespace("NetArchTest.TestStructure.NameMatching") 
                      & HaveNameEndingWith("A1"))
                .GetTypes();

            Assert.Single(result); // One type found
            Assert.Contains<Type>(typeof(ClassA1), result);
        }

        [Fact(DisplayName = "Types can be selected if their name does not have a specific end.")]
        public void DoNotHaveNameEnding_MatchesFound_ClassesSelected()
        {
            var result = Types
                .InAssembly(Assembly.GetAssembly(typeof(ClassA1)))
                .That(ResideInNamespace("NetArchTest.TestStructure.NameMatching")
                & !HaveNameEndingWith("A1"))
                .GetTypes();

            Assert.Equal(4, result.Count()); // three types found
            Assert.Contains<Type>(typeof(ClassA2), result);
            Assert.Contains<Type>(typeof(ClassA3), result);
            Assert.Contains<Type>(typeof(ClassB1), result);
            Assert.Contains<Type>(typeof(ClassB2), result);
        }

        [Fact(DisplayName = "Types can be selected by a regular expression.")]
        public void HaveNameMatching_MatchesFound_ClassesSelected()
        {
            var result = Types
                .InAssembly(Assembly.GetAssembly(typeof(ClassA1)))
                .That(ResideInNamespace("NetArchTest.TestStructure.NameMatching") 
                    & HaveNameMatching(@"Class\w1"))
                .GetTypes();

            Assert.Equal(2, result.Count()); // Two types found
            Assert.Contains<Type>(typeof(ClassA1), result);
            Assert.Contains<Type>(typeof(ClassB1), result);
        }

        [Fact(DisplayName = "Types can be selected if they do not conform to a regular expression.")]
        public void DoNotHaveNameMatching_MatchesFound_ClassesSelected()
        {
            var result = Types
                .InAssembly(Assembly.GetAssembly(typeof(ClassA1)))
                .That(ResideInNamespace("NetArchTest.TestStructure.NameMatching") &
                      !HaveNameMatching(@"Class\w1")     )
                .GetTypes();

            Assert.Equal(3, result.Count()); // Three types found
            Assert.Contains<Type>(typeof(ClassA2), result);
            Assert.Contains<Type>(typeof(ClassA3), result);
            Assert.Contains<Type>(typeof(ClassB2), result);
        }

        [Fact(DisplayName = "Types can be selected by a the presence of a custom attribute.")]
        public void HaveCustomAttribute_MatchesFound_ClassesSelected()
        {
            var result = Types
                .InAssembly(Assembly.GetAssembly(typeof(ClassA1)))
                .That(ResideInNamespace("NetArchTest.TestStructure.CustomAttributes")
                & HaveCustomAttribute(typeof(ClassCustomAttribute))).GetTypes();

            Assert.Single(result); // One type found
            Assert.Contains<Type>(typeof(AttributePresent), result);
        }

        [Fact(DisplayName = "Types can be selected by the absence of a custom attribute.")]
        public void DoNotHaveCustomAttribute_MatchesFound_ClassesSelected()
        {
            var result = Types
                .InAssembly(Assembly.GetAssembly(typeof(ClassA1)))
                .That(ResideInNamespace("NetArchTest.TestStructure.CustomAttributes")
                    & !HaveCustomAttribute(typeof(ClassCustomAttribute)))
                .GetTypes();

            Assert.Equal(2, result.Count()); // Two types found
            Assert.Contains<Type>(typeof(NoAttributes), result);
            Assert.Contains<Type>(typeof(ClassCustomAttribute), result);
        }

        [Fact(DisplayName = "Types can be selected if they inherit from a type.")]
        public void Inherit_MatchesFound_ClassesSelected()
        {
            var result = Types
                .InAssembly(Assembly.GetAssembly(typeof(ClassA1)))
                .That(ResideInNamespace("NetArchTest.TestStructure.Inheritance") 
                    & Inherit(typeof(BaseClass)))
                .GetTypes();

            Assert.Equal(2, result.Count()); // Two types found
            Assert.Contains<Type>(typeof(DerivedClass), result);
            Assert.Contains<Type>(typeof(DerivedDerivedClass), result);
        }

        [Fact(DisplayName = "Types can be selected if they do not inherit from a type.")]
        public void DoNotInherit_MatchesFound_ClassesSelected()
        {
            var result = Types
                .InAssembly(Assembly.GetAssembly(typeof(ClassA1)))
                .That(ResideInNamespace("NetArchTest.TestStructure.Inheritance")
                    & !Inherit(typeof(BaseClass)))
                .GetTypes();

            Assert.Equal(2, result.Count()); // Two types found
            Assert.Contains<Type>(typeof(BaseClass), result);
            Assert.Contains<Type>(typeof(NotDerivedClass), result);
        }

        [Fact(DisplayName = "Types can be selected if they implement an interface.")]
        public void ImplementInterface_MatchesFound_ClassesSelected()
        {
            var result = Types
                .InAssembly(Assembly.GetAssembly(typeof(ClassA1)))
                .That(ResideInNamespace("NetArchTest.TestStructure.Interfaces")
                    & ImplementInterface(typeof(IExample)))
                .GetTypes();

            Assert.Single(result); // One type found
            Assert.Contains<Type>(typeof(ImplementsExampleInterface), result);
        }

        [Fact(DisplayName = "Types can be selected if they do not implement an interface.")]
        public void DoNotImplementInterface_MatchesFound_ClassesSelected()
        {
            var result = Types
                .InAssembly(Assembly.GetAssembly(typeof(ClassA1)))
                .That(ResideInNamespace("NetArchTest.TestStructure.Interfaces")
                    & !ImplementInterface(typeof(IExample)))
                .GetTypes();

            Assert.Equal(2, result.Count()); // Two types found
            Assert.Contains(typeof(IExample), result);
            Assert.Contains(typeof(DoesNotImplementInterface), result);
        }

        [Fact(DisplayName = "Types can be selected if they are abstract.")]
        public void AreAbstract_MatchesFound_ClassesSelected()
        {
            var result = Types
                .InAssembly(Assembly.GetAssembly(typeof(ClassA1)))
                .That(ResideInNamespace("NetArchTest.TestStructure.Abstract") &
                      AreAbstract())
                .GetTypes();

            Assert.Single(result); // One type found
            Assert.Contains<Type>(typeof(AbstractClass), result);
        }

        [Fact(DisplayName = "Types can be selected if they are not abstract.")]
        public void AreNotAbstract_MatchesFound_ClassesSelected()
        {
            var result = Types
                .InAssembly(Assembly.GetAssembly(typeof(ClassA1)))
                .That(ResideInNamespace("NetArchTest.TestStructure.Abstract") &
                    !AreAbstract())
                .GetTypes();

            Assert.Single(result); // One type found
            Assert.Contains<Type>(typeof(ConcreteClass), result);
        }

        [Fact(DisplayName = "Types can be selected if they are classes.")]
        public void AreClasses_MatchesFound_ClassesSelected()
        {
            var result = Types
                .InAssembly(Assembly.GetAssembly(typeof(ClassA1)))
                .That(ResideInNamespace("NetArchTest.TestStructure.Classes")
                    & AreClasses())
                .GetTypes();

            Assert.Single(result); // One type found
            Assert.Contains<Type>(typeof(ExampleClass), result);
        }

        [Fact(DisplayName = "Types can be selected if they are not classes.")]
        public void AreNotClasses_MatchesFound_ClassesSelected ()
        {
            var result = Types
                .InAssembly(Assembly.GetAssembly(typeof(ClassA1)))
                .That(ResideInNamespace("NetArchTest.TestStructure.Classes")
                    & !AreClasses())
                .GetTypes();

            Assert.Single(result); // One type found
            Assert.Contains<Type>(typeof(IExampleInterface), result);
        }

        [Fact(DisplayName = "Types can be selected if they have generic parameters.")]
        public void AreGeneric_MatchesFound_ClassesSelected()
        {
            var result = Types
                .InAssembly(Assembly.GetAssembly(typeof(ClassA1)))
                .That(ResideInNamespace("NetArchTest.TestStructure.Generic")
                    & AreGeneric())
                .GetTypes();

            Assert.Single(result); // One type found
            Assert.Contains<Type>(typeof(GenericType<>), result);
        }

        [Fact(DisplayName = "Types can be selected if they do not have generic parameters.")]
        public void AreNotGeneric_MatchesFound_ClassesSelected()
        {
            var result = Types
                .InAssembly(Assembly.GetAssembly(typeof(ClassA1)))
                .That(ResideInNamespace("NetArchTest.TestStructure.Generic")
                & !AreGeneric())
                .GetTypes();

            Assert.Single(result); // One type found
            Assert.Contains(typeof(NonGenericType), result);
        }

        [Fact(DisplayName = "Types can be selected if they are interfaces.")]
        public void AreInterfaces_MatchesFound_ClassesSelected()
        {
            var result = Types
                .InAssembly(Assembly.GetAssembly(typeof(ClassA1)))
                .That(ResideInNamespace("NetArchTest.TestStructure.Classes")
                & AreInterfaces())
                .GetTypes();

            Assert.Single(result); // One type found
            Assert.Contains<Type>(typeof(IExampleInterface), result);
        }

        [Fact(DisplayName = "Types can be selected if they are not interfaces.")]
        public void AreNotInterfaces_MatchesFound_ClassesSelected ()
        {
            var result = Types
                .InAssembly(Assembly.GetAssembly(typeof(ClassA1)))
                .That(ResideInNamespace("NetArchTest.TestStructure.Classes")
                & !AreInterfaces())
                .GetTypes();

            Assert.Single(result); // One type found
            Assert.Contains<Type>(typeof(ExampleClass), result);
        }

        [Fact(DisplayName = "Types can be selected if they are nested.")]
        public void AreNested_MatchesFound_ClassesSelected()
        {
                var result = Types
                .InAssembly(Assembly.GetAssembly(typeof(ClassA1)))
                .That(ResideInNamespace("NetArchTest.TestStructure.Nested") 
                      & AreNested())
                .GetTypes();

            Assert.Single(result); // One type found
            Assert.Contains<Type>(typeof(NestedContainer.NestedClass), result);
        }

        [Fact(DisplayName = "Types can be selected if they are not nested.")]
        public void AreNotNested_MatchesFound_ClassesSelected()
        {
            var result = Types
                .InAssembly(Assembly.GetAssembly(typeof(ClassA1)))
                .That(ResideInNamespace("NetArchTest.TestStructure.Nested")
                    & !AreNested())
                .GetTypes();

            Assert.Equal(2, result.Count()); // Two types found
            Assert.Contains(typeof(NotNested), result);
            Assert.Contains(typeof(NestedContainer), result);
        }

        [Fact(DisplayName = "Types can be selected for being declared as public.")]
        public void ArePublic_MatchesFound_ClassSelected()
        {
            var result = Types
                .InAssembly(Assembly.GetAssembly(typeof(ClassA1)))
                .That(ResideInNamespace("NetArchTest.TestStructure.Scope")
                & ArePublic())
                .GetTypes();

            Assert.Single(result); // One result
            Assert.Contains<Type>(typeof(PublicClass), result);
        }

        [Fact(DisplayName = "Types can be selected for not being declared as public.")]
        public void AreNotPublic_MatchesFound_ClassSelected ()
        {
            var result = Types
                .InAssembly(Assembly.GetAssembly(typeof(ClassA1)))
                .That(ResideInNamespace("NetArchTest.TestStructure.Scope") 
                      & !ArePublic())
                .GetTypes();

            Assert.Single(result); // One result
            Assert.Contains<Type>(typeof(InternalClass), result);
        }

        [Fact(DisplayName = "Types can be selected for being declared as sealed.")]
        public void AreSealed_MatchesFound_ClassSelected()
        {
            var result = Types
                .InAssembly(Assembly.GetAssembly(typeof(ClassA1)))
                .That(ResideInNamespace("NetArchTest.TestStructure.Sealed")
                    & AreSealed())
                .GetTypes();

            Assert.Single(result); // One result
            Assert.Contains<Type>(typeof(SealedClass), result);
        }

        [Fact(DisplayName = "Types can be selected for not being declared as sealed.")]
        public void AreNotSealed_MatchesFound_ClassSelected()
        {
            var result = Types
                .InAssembly(Assembly.GetAssembly(typeof(ClassA1)))
                .That(ResideInNamespace("NetArchTest.TestStructure.Sealed") &
                      !AreSealed())
                .GetTypes();

            Assert.Single(result); // One result
            Assert.Contains<Type>(typeof(NotSealedClass), result);
        }

        [Fact(DisplayName = "Types can be selected if they reside in a namespace.")]
        public void ResideInNamespace_MatchesFound_ClassSelected()
        {
            var result = Types
                .InAssembly(Assembly.GetAssembly(typeof(ClassA1)))
                .That(ResideInNamespace("NetArchTest.TestStructure.NameMatching.Namespace1"))
                .GetTypes();

            Assert.Equal(3, result.Count()); // Three types found
            Assert.Contains<Type>(typeof(ClassA1), result);
            Assert.Contains<Type>(typeof(ClassA2), result);
            Assert.Contains<Type>(typeof(ClassB1), result);
        }
        
        [Fact(DisplayName = "Types can be selected if they reside in a namespace.")]
        public void CustomizedResideInNamespace_MatchesFound_ClassSelected()
        {
            var result = Types
                .InAssembly(Assembly.GetAssembly(typeof(ClassA1)))
                .That(ResideInNamespace("NetArchTest.TestStructure.NameMatching.Namespace1"))
                .GetTypes();

            Assert.Equal(3, result.Count()); // Three types found
            Assert.Contains<Type>(typeof(ClassA1), result);
            Assert.Contains<Type>(typeof(ClassA2), result);
            Assert.Contains<Type>(typeof(ClassB1), result);
        }

        [Fact(DisplayName = "Types can be selected if they do not reside in a namespace.")]
        public void DoNotResideInNamespace_MatchesFound_ClassSelected()
        {
            var result = Types
                .InAssembly(Assembly.GetAssembly(typeof(ClassA1)))
                .That(ResideInNamespace("NetArchTest.TestStructure.NameMatching") 
                      & !ResideInNamespace("NetArchTest.TestStructure.NameMatching.Namespace2"))
                .GetTypes();

            Assert.Equal(3, result.Count()); // Three types found
            Assert.Contains<Type>(typeof(ClassA1), result);
            Assert.Contains<Type>(typeof(ClassA2), result);
            Assert.Contains<Type>(typeof(ClassB1), result);
        }        
        
        [Fact(DisplayName = "Types can be selected if they do not reside in a namespace.")]
        public void CustomizedDoNotResideInNamespace_MatchesFound_ClassSelected()
        {
            var result = Types
                .InAssembly(Assembly.GetAssembly(typeof(ClassA1)))
                .That(ResideInNamespace("NetArchTest.TestStructure.NameMatching")
                & !ResideInNamespace("NetArchTest.TestStructure.NameMatching.Namespace2"))
                .GetTypes();

            Assert.Equal(3, result.Count()); // Three types found
            Assert.Contains<Type>(typeof(ClassA1), result);
            Assert.Contains<Type>(typeof(ClassA2), result);
            Assert.Contains<Type>(typeof(ClassB1), result);
        }

        [Fact(DisplayName = "Selecting by namespace will return types in nested namespaces.")]
        public void ResideInNamespace_Nested_AllClassReturned()
        {
            var result = Types
                .InAssembly(Assembly.GetAssembly(typeof(ClassA1)))
                .That(ResideInNamespace("NetArchTest.TestStructure.NameMatching"))
                .GetTypes();

            // Should return all the types that are in three nested namespaces
            Assert.Equal(5, result.Count()); // Five types found
            Assert.Contains<Type>(typeof(ClassA1), result);
            Assert.Contains<Type>(typeof(ClassA2), result);
            Assert.Contains<Type>(typeof(ClassA3), result);
            Assert.Contains<Type>(typeof(ClassB1), result);
            Assert.Contains<Type>(typeof(ClassB2), result);
        }

        [Fact(DisplayName = "Types can be selected if they have a dependency on another type.")]
        public void HaveDepencency_MatchesFound_ClassSelected()
        {
            var result = Types
                .InAssembly(Assembly.GetAssembly(typeof(ClassA1)))
                .That(ResideInNamespace("NetArchTest.TestStructure.Dependencies.Implementation")
                      & HaveDependencyOn("NetArchTest.TestStructure.Dependencies.ExampleDependency"))
                .GetTypes();

            Assert.Single(result); // Only one type found
            Assert.Equal<Type>(typeof(HasDependency), result.First()); // The correct type found
        }

        [Fact(DisplayName = "Types can be selected if they do not have a dependency on another type.")]
        public void DoNotHaveDepencency_MatchesFound_ClassSelected()
        {
            var result = Types
                .InAssembly(Assembly.GetAssembly(typeof(ClassA1)))
                .That(ResideInNamespace("NetArchTest.TestStructure.Dependencies.Implementation")
                    & !HaveDependencyOn("NetArchTest.TestStructure.Dependencies.ExampleDependency"))
                .GetTypes();

            Assert.Single(result); // Only one type found
            Assert.Equal<Type>(typeof(NoDependency), result.First()); // The correct type found
        }

    }
}
