using System;
using System.Collections.Generic;
using UnityEngine;

public static class ServiceLocator
{
    private static Dictionary<Type, MonoBehaviour> servicesDict = new Dictionary<Type, MonoBehaviour>();

    public static void Register<T>(T service) where T : MonoBehaviour
    {
        servicesDict.Add(typeof(T), service);
    }

    public static T Get<T>() where T : MonoBehaviour
    {
        return (T)servicesDict[typeof(T)];
    }    

    public static void ClearServices()
    {
        servicesDict.Clear();
    }
}