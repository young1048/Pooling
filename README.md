
# Istruction:
==============================================================

* *1)* Create a Pool containers: Tools/Create

![Create](https://smallthinggames.com/UnityTutorials/Pooling/s1.png)






* *2)* Create a database: Tools/Create database

![Create Database](https://smallthinggames.com/UnityTutorials/Pooling/s2.png)






* *3)* Create a object to insert database and drag&drop in the database section inside inspector

The object must be a Util_PoolObject to see inside system database. So when you create the script override mono class with
Util_PoolObject.

```c#
using UnityEngine;
using System.Collections;

// NOTE: Every pool object must be a Util_PoolObject
public class TestObject : Util_PoolObject {


}
```

* *4)* Set a TAG in the database object

![Create Tag](https://smallthinggames.com/UnityTutorials/Pooling/s3.png)






# Create Object
==============================================================

After created the database and container database, you can create you object with a simple function:

```c#
GameObject o = Util_PoolManagerDatabase.GetObject("MYPOOL/MyObject");
```

Where *MYPOOL* is the tag setted inside database and *MyObject* is the object drop in database inspector.



# Video tutorial
==============================================================

https://smallthinggames.com/UnityTutorials/Pooling/Pooling.mp4



# Unity Package
==============================================================

http://smallthinggames.com/UnityTutorials/Pooling/Pooling.unitypackage


# Contributing

Please read our contributing guidelines prior to submitting a Pull Request.

# License

All files are released under the MIT License:

    The MIT License (MIT)

    Copyright (c) 2013-2016 PayPal Holdings, Inc.

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in
    all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    THE SOFTWARE.


## Donation
==============================================================

If this tutorial it was helpful for you, please make a donation via **PayPal** to buy a beer :D

PayPal account: **ubiksoft@gmail.com**

*Thanks!* :)

