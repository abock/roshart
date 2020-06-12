# Roshart

_**TL;DR: don't use this. Period.**_

Roshart serves [CatOverflow.com](https://catoverflow.com), [DogOverflow.com](https://dogoverflow.com), and [OtterOverflow.com](https://otteroverflow.com).

After running mostly 24x7 for 8 years via Nginx (and originally Lighttpd),
FastCGI, and Ruby, it finally broke following a server OS upgrade in June 2020,
due to a very outdated ruby-fcgi gem.

This original Ruby server was called _Shart_. Don't ask.

In an effort to keep the content alive, I wrote a very quick ASP.NET Core
version without the ability to upload new content. That is, Read-Only-Shart,
or, _Roshart_.

## Is Roshart Running?

If you see GIFs it is...

### CatOverflow.com

![Random Cat GIF](https://catoverflow.com/api/query?order=random&render)

### DogOverflow.com

![Random Dog GIF](https://dogoverflow.com/api/query?order=random&render)

### OtterOverflow.com

![Random Otter GIF](https://otteroverflow.com/api/query?order=random&render)