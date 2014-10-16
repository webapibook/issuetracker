issuetracker
============
This repo contains the sample API used in the book.

# Implementation
This is a hypermedia API for managing issues. Using it you can retrieve, create, modify and transition the state of issues. is implemented with ASP.NET Web API. You can find out all the details on the design and implementation in the book [here] (http://chimera.labs.oreilly.com/books/1234000001708/ch07.html)

# Try it out
This API is hosted live in the cloud via [Azure] (http://azure.net). Here is a simple walkthrough showing how you can navigate the API. You can use this same walkthrough against the local API by subsituting the root URL for your local machine and using the links returned to you rather than the azure links below. (Hence the value of hypermedia :-) )

One other thing to note. As this is a single instance hosted in the cloud it is possible that this walkthrough may not yield exactly the same results if multiple folks are hitting the API at the same time. However, the state is all in memory, so after a few mins of non-usage, the site will get torn down and the data reset to what is below.

## The entry point
First hit the root entry point usign the following command:

```text
curl -H "accept:application/vnd.collection+json" http://webapibook-issuetracker.azurewebsites.net/issue
```

This will return the full list of issues:

```javascript
{
  "collection": {
    "href": "http://webapibook-issuetracker.azurewebsites.net:80/issue",
    "links": [
      {
        "rel": "profile",
        "href": "http://webapibook.net/profile"
      }
    ],
    "items": [
      {
        "href": "http://webapibook-issuetracker.azurewebsites.net/issue/1",
        "data": [
          {
            "name": "Description",
            "value": "This is an issue"
          },
          {
            "name": "Status",
            "value": "Open"
          },
          {
            "name": "Title",
            "value": "An issue"
          }
        ],
        "links": [
          {
            "rel": "http://webapibook.net/profile#transition",
            "href": "http://webapibook-issuetracker.azurewebsites.net/issueprocessor/1?action=transition"
          },
          {
            "rel": "http://webapibook.net/profile#close",
            "href": "http://webapibook-issuetracker.azurewebsites.net/issueprocessor/1?action=close"
          }
        ]
      },
      {
        "href": "http://webapibook-issuetracker.azurewebsites.net/issue/2",
        "data": [
          {
            "name": "Description",
            "value": "This is a another issue"
          },
          {
            "name": "Status",
            "value": "Closed"
          },
          {
            "name": "Title",
            "value": "Another Issue"
          }
        ],
        "links": [
          {
            "rel": "http://webapibook.net/profile#transition",
            "href": "http://webapibook-issuetracker.azurewebsites.net/issueprocessor/2?action=transition"
          },
          {
            "rel": "http://webapibook.net/profile#open",
            "href": "http://webapibook-issuetracker.azurewebsites.net/issueprocessor/2?action=open"
          }
        ]
      }
    ],
    "queries": [
      {
        "rel": "http://webapibook.net/profile#search",
        "href": "/issue",
        "prompt": "Issue search",
        "data": [
          {
            "name": "SearchText",
            "prompt": "Text to match against Title and Description"
          }
        ]
      }
    ],
    "template": {
      "data": [
        {
          "name": "Description",
          "prompt": "Description for the issue"
        },
        {
          "name": "Status",
          "prompt": "Status of the issue (Open or Closed)"
        },
        {
          "name": "Title",
          "prompt": "Title for the issue"
        }
      ]
    }
  }
  ```
## Retrieving a specific issue
Notice the first `issue` is in an open state. Let's retrieve that specific `issue` by following its `href`.

```text
curl http://webapibook-issuetracker.azurewebsites.net/issue/1
```

This returns the issue detail in JSON.

```javascript
{
  "id": "1",
  "title": "An issue",
  "description": "This is an issue",
  "status": "Open",
  "links": [
    {
      "rel": "self",
      "href": "http://webapibook-issuetracker.azurewebsites.net/issue/1"
    },
    {
      "rel": "http://webapibook.net/profile#transition",
      "href": "http://webapibook-issuetracker.azurewebsites.net/issueprocessor/1?action=transition",
      "action": "transition"
    },
    {
      "rel": "http://webapibook.net/profile#close",
      "href": "http://webapibook-issuetracker.azurewebsites.net/issueprocessor/1?action=close",
      "action": "close"
    }
  ]
}
```
## Searching for items
You can also search for a specific issue. If you look at the template in the first Collection+JSON response you can see there is a search query.

```javascript
"queries": [
  {
    "rel": "http://webapibook.net/profile#search",
    "href": "/issue",
    "prompt": "Issue search",
    "data": [
      {
        "name": "SearchText",
        "prompt": "Text to match against Title and Description"
      }
    ]
  }
]
```

CJ queries supply parameters that are passed in the URI as part of the query string. In this case you can see the `href` is at the `issue` resource and that there is a 'SearchText' query param. Let's see how we can use that to search for issue 2. Queries return CJ documents.

```text
curl -H "accept:application/vnd.collection+json" http://webapibook-issuetracker.azurewebsites.net/issue?searchtext=another
```

Sure enough this returns a CJ document with the issue named "This is another issue".

```javascript
{
  "collection": {
    "href": "http://webapibook-issuetracker.azurewebsites.net:80/issue?searchtext=another",
    "links": [
      {
        "rel": "profile",
        "href": "http://webapibook.net/profile"
      }
    ],
    "items": [
      {
        "href": "http://webapibook-issuetracker.azurewebsites.net/issue/2",
        "data": [
          {
            "name": "Description",
            "value": "This is a another issue"
          },
          {
            "name": "Status",
            "value": "Closed"
          },
          {
            "name": "Title",
            "value": "Another Issue"
          }
        ],
        "links": [
          {
            "rel": "http://webapibook.net/profile#transition",
            "href": "http://webapibook-issuetracker.azurewebsites.net/issueprocessor/2?action=transition"
          },
          {
            "rel": "http://webapibook.net/profile#open",
            "href": "http://webapibook-issuetracker.azurewebsites.net/issueprocessor/2?action=open"
          }
        ]
      }
    ],
    "queries": [
      {
        "rel": "http://webapibook.net/profile#search",
        "href": "/issue",
        "prompt": "Issue search",
        "data": [
          {
            "name": "SearchText",
            "prompt": "Text to match against Title and Description"
          }
        ]
      }
    ],
    "template": {
      "data": [
        {
          "name": "Description",
          "prompt": "Description for the issue"
        },
        {
          "name": "Status",
          "prompt": "Status of the issue (Open or Closed)"
        },
        {
          "name": "Title",
          "prompt": "Title for the issue"
        }
      ]
    }
  }
}
```
## Using an action link
Next you can see how to transition the state of the `issue` using the hypermedia (action links) within. This `issue` has a state of `closed`. You can open it by posting to the href of the open link:

```javascript
"links": [
    {
      "rel": "http://webapibook.net/profile#transition",
      "href": "http://webapibook-issuetracker.azurewebsites.net/issueprocessor/2?action=transition"
    },
    {
      "rel": "http://webapibook.net/profile#open",
      "href": "http://webapibook-issuetracker.azurewebsites.net/issueprocessor/2?action=open"
    }
  ]
```

Use the following command:

```text
curl -d '' http://webapibook-issuetracker.azurewebsites.net/issueprocessor/2?action=open
```

This will not return a response. Checking the status of the issue will show that it has indeed been opened.

```text
curl http://webapibook-issuetracker.azurewebsites.net/issue/2
```

With this result:

```javascript
{
  "id": "2",
  "title": "Another Issue",
  "description": "This is a another issue",
  "status": "Open",
  "links": [
    {
      "rel": "self",
      "href": "http://webapibook-issuetracker.azurewebsites.net/issue/2"
    },
    {
      "rel": "http://webapibook.net/profile#transition",
      "href": "http://webapibook-issuetracker.azurewebsites.net/issueprocessor/2?action=transition",
      "action": "transition"
    },
    {
      "rel": "http://webapibook.net/profile#close",
      "href": "http://webapibook-issuetracker.azurewebsites.net/issueprocessor/2?action=close",
      "action": "close"
    }
  ]
}
```

## Validation
Finally you can see the API validation by attempting to use the `open` link again, which is invalid. The -v param specifies verbose so you can see the status code as well.

```text
curl -v -d '' http://webapibook-issuetracker.azurewebsites.net/issueprocessor/2?action=open
```

You should see the following response indicating an error. 

```text
> User-Agent: curl/7.30.0
> Host: webapibook-issuetracker.azurewebsites.net
> Accept: */*
> Content-Length: 0
> Content-Type: application/x-www-form-urlencoded
>
< HTTP/1.1 400 Bad Request
< Cache-Control: no-cache
< Pragma: no-cache
< Content-Length: 45
< Content-Type: application/json; charset=utf-8
< Expires: -1
* Server Microsoft-IIS/8.0 is not blacklisted
< Server: Microsoft-IIS/8.0
< X-AspNet-Version: 4.0.30319
< X-Powered-By: ASP.NET
< Set-Cookie: ARRAffinity=ccfec91eb2c9455382c57d9f048af6af6ab3ef6946377d30ef0a62a0fc14e489;Path=/;Domain=webapibook-issuetracker.azurewebsites.net
< Date: Thu, 16 Oct 2014 06:32:49 GMT
* HTTP error before end of send, stop sending
<
{
  "message": "Action 'open' is invalid"
* Closing connection 0
}
```





