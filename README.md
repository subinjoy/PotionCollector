Unity Version Used : 6.0(6000.0.49f1)
Package Name : com.Subin.PotionCollector
Version 0.1.0 (1)
Database URL : https://potioncollector-dcb4f-default-rtdb.firebaseio.com/
Addressables URL :storage.bunnycdn.com
Repository URL :https://github.com/subinjoy/PotionCollector

1. Game Architecture

   The system utilizes ScriptableObject assets to define events. Any class can raise an event without knowing which other classes listen to it. 
   This prevents hardcoding dependencies.
   Event System Foundation consists of the generic BaseGameEvent<T>, which handles listener registration and the Raise(T data) function, 
   and the IGameEventListener<T> interface, which is implemented by any class designed to receive event data.
   All communication data is standardized within EventData.cs, which contains structs like SessionData and GameEndData.
   The Game Logic Layer is driven by the GameFlowController.cs, which initiates sessions by generating unique IDs and raising the GameStartedEvent, 
   and calculates scores by listening to the PotionCollectedEvent, and concludes the game by raising the GameEndedEvent with the final GameEndData
   Custom ScriptableObject for Potion Data holds the data for each potion type and serve as the reference for Addressables.

    Potions in the Game: RED Potion  :Score 15  
                         GREEN Potion :Score 10
                         MAGIC POTION : Score20
  Values are set using Scriptable Object and Custom editor

2.Game Flow
     
    Start: GameFlowController generates a sessionId and sessionStartTime and raises a GameStartedEvent (containing SessionData).
    Tracking: FirebaseManager listens to GameStartedEvent and logs the game_start analytics event.
    Gameplay: The player interacts with Potion objects, which trigger the PotionCollectedEvent (containing PotionCollectionData).
    Analytics: FirebaseManager listens to PotionCollectedEvent and logs the potion_collected analytics event.
    Game End: When the timer expires, GameFlowController raises a GameEndedEvent (containing GameEndData).
    FireBase: FirebaseManager listens to GameEndedEvent, saves the final score to the Realtime Database, and logs the game_end analytics event.
    

3.Addressables 

   Addressables are used in this project to manage Different Potion Assets(Red,Green and Magical Potion)
   Addressables are Hosted in the following URL : storage.bunnycdn.com
   Each potion has been given unique labels.
   Usage: When a potion is spawned, the spawning script uses the Addressables.LoadAssetAsync<T>(label) call to fetch the asset at runtime.
   Content Building: Before building the Android APK, you must run the Addressables content build: Window > Asset Management > Addressables > Groups > Build > New Build > Default Build Script.
    
4.Firebase Setup Guide (For storing User data to Firebase) 
   Download Firebase SDK for unity from https://firebase.google.com/docs/unity/setup
   Install the following package using Assets>Import>CustomPackage and then import the following:
      FirebaseAuth.UnityPackage  
      FirebaseDataBase.UnityPackage
      FirebaseAnalytics.UnityPackage
 
  The google-services.Json file is present in AssetsFolder.
  Dependency Resolution:In Unity, navigate to Assets > External Dependency Manager > Android Resolver > Force Resolve. This ensures the necessary native libraries are linked.

  If google-services.Json fails to configure,then use the following steps:
     Database URL : https://potioncollector-dcb4f-default-rtdb.firebaseio.com/
     Open GamePlayScene and open Firbase Component and Make sure the database URL is added in the database URL Field in the inspector.
  
 Details saved in the LeaderBoard:FinalScore
                                  PlayerName
                                  SessionStartTime
                                  SessionStartTime
                                  Player Name
                                  UserID
 
5.Guide for taking the Build
   
   Firebase Dependency Check:Go to Assets > External Dependency Manager > Android Resolver > Force Resolve.
   Addressable Build : Go to Window > Asset Management > Addressables > Groups. Select Build > New Build > Default Build Script.
   Go to File > Build Settings and click Player Settings
   Check the Bundle name : "com.Subin.PotionCollector"
   Check and verify  the version "0.1.0(1)"
   Minimum API Level: Set to Android 21 (Lollipop) or higher
   Maximum API Level :Automatically set to highest
   Resolution : Potrait
   Click on Build profiles and Makesure that "GamePlay Scene" is been added in the scenes list
   Click on build to build the apk file.
