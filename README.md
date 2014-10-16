issuetracker
============
This repo contains the sample API used in the book.

# Implementation
This is a hypermedia API for managing issues. Using it you can retrieve, create, modify and transition the state of issues. is implemented with ASP.NET Web API. You can find out all the details on the design and implementation in the book [here] (http://chimera.labs.oreilly.com/books/1234000001708/ch07.html)

# Try it out
This API is hosted live in the cloud via [Azure] (http://azure.net). Here is a simple walkthrough showing how you can navigate the API.

## The entry point
First hit the root entry point usign the following command:

```text
curl -H "accept:application/vnd.collection+jso" http://webapibook-issuetracker.azurewebsites.net/issue
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






