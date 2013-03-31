﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using WebApiBook.IssueTrackerApi.Infrastructure;
using Should;

namespace WebApiBook.IssueTrackerApi.Tests
{
    public class UriExtensionsTests
    {
        public class TheGetQueryStringObjectMethod
        {
            [Fact]
            public void ShouldHandleQueryStringWithSingleValuesPerKey()
            {
                var request = new Uri("http://localhost/test.com?foo=value1&bar=value2");
                var obj = request.GetQueryStringObject();
                var foo = (string) obj.foo;
                var bar = (string) obj.bar;

                foo.ShouldEqual("value1");
                bar.ShouldEqual("value2");
            }

            [Fact]
            public void ShouldHandleQueryStringWithMultipleValuesPerKey()
            {
                var request = new Uri("http://localhost/test.com?foo=value1&foo=value2");
                var obj = request.GetQueryStringObject();
                var foos = new List<string>((IEnumerable<string>) obj.foo);

                foos[0].ShouldEqual("value1");
                foos[1].ShouldEqual("value2");
            }
        }
    }
}
