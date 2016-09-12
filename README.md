
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

