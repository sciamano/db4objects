package com.db4o.f1.chapter1;


import java.io.File;

import com.db4o.Db4o;
import com.db4o.ObjectContainer;
import com.db4o.ObjectSet;
import com.db4o.f1.Util;


public class FirstStepsExample extends Util {    
    public static void main(String[] args) {
        new File(Util.DB4OFILENAME).delete();
        accessDb4o();
        new File(Util.DB4OFILENAME).delete();
        ObjectContainer db=Db4o.openFile(Util.DB4OFILENAME);
        try {
            storeFirstPilot(db);
            storeSecondPilot(db);
            retrieveAllPilots(db);
            retrievePilotByName(db);
            retrievePilotByExactPoints(db);
            updatePilot(db);
            deleteFirstPilotByName(db);
            deleteSecondPilotByName(db);
        }
        finally {
            db.close();
        }
    }
    
    public static void accessDb4o() {
        ObjectContainer db=Db4o.openFile(Util.DB4OFILENAME);
        try {
            // do something with db4o
        }
        finally {
            db.close();
        }
    }
    
    public static void storeFirstPilot(ObjectContainer db) {
        Pilot pilot1=new Pilot("Michael Schumacher",100);
        db.set(pilot1);
        System.out.println("Stored "+pilot1);
    }

    public static void storeSecondPilot(ObjectContainer db) {
        Pilot pilot2=new Pilot("Rubens Barrichello",99);
        db.set(pilot2);
        System.out.println("Stored "+pilot2);
    }

    public static void retrieveAllPilotQBE(ObjectContainer db) {
        Pilot proto=new Pilot(null,0);
        ObjectSet result=db.get(proto);
        listResult(result);
    }
    
    public static void retrieveAllPilots(ObjectContainer db) {
        ObjectSet result=db.get(Pilot.class);
        listResult(result);
    }

    public static void retrievePilotByName(ObjectContainer db) {
        Pilot proto=new Pilot("Michael Schumacher",0);
        ObjectSet result=db.get(proto);
        listResult(result);
    }
    
    public static void retrievePilotByExactPoints(ObjectContainer db) {
        Pilot proto=new Pilot(null,100);
        ObjectSet result=db.get(proto);
        listResult(result);
    }

    public static void updatePilot(ObjectContainer db) {
        ObjectSet result=db.get(new Pilot("Michael Schumacher",0));
        Pilot found=(Pilot)result.next();
        found.addPoints(11);
        db.set(found);
        System.out.println("Added 11 points for "+found);
        retrieveAllPilots(db);
    }

    public static void deleteFirstPilotByName(ObjectContainer db) {
        ObjectSet result=db.get(new Pilot("Michael Schumacher",0));
        Pilot found=(Pilot)result.next();
        db.delete(found);
        System.out.println("Deleted "+found);
        retrieveAllPilots(db);
    }

    public static void deleteSecondPilotByName(ObjectContainer db) {
        ObjectSet result=db.get(new Pilot("Rubens Barrichello",0));
        Pilot found=(Pilot)result.next();
        db.delete(found);
        System.out.println("Deleted "+found);
        retrieveAllPilots(db);
    }
}
