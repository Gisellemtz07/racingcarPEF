using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

[Serializable]
public enum UserType
{
    Player,
    Therapist
}

[Serializable]
public class UserCredentials
{
    public string username;
    public string passwordHash;
}

[Serializable]
public class StoryPlanStep
{
    public string levelName;
    public int laps = 1;
    public int order;
}

[Serializable]
public class StoryPlanData
{
    public List<StoryPlanStep> steps = new List<StoryPlanStep>();
}

[Serializable]
public class UserRecord
{
    public UserType userType = UserType.Player;
    public UserCredentials credentials = new UserCredentials();
    public StoryPlanData storyPlan = new StoryPlanData();

    // Legacy fields kept for backwards compatibility with older saves.
    public string username;
    public string passwordHash;

    public void EnsureConsistency()
    {
        if (credentials == null) credentials = new UserCredentials();
        if (storyPlan == null) storyPlan = new StoryPlanData();

        if (string.IsNullOrEmpty(credentials.username) && !string.IsNullOrEmpty(username))
        {
            credentials.username = username;
        }

        if (string.IsNullOrEmpty(credentials.passwordHash) && !string.IsNullOrEmpty(passwordHash))
        {
            credentials.passwordHash = passwordHash;
        }

        // keep legacy fields synced to always write the latest structure
        username = credentials.username;
        passwordHash = credentials.passwordHash;
    }
}

[Serializable]
class UserDatabaseData
{
    public List<UserRecord> users = new List<UserRecord>();
}

public static class UserDatabase
{
    static UserDatabaseData data = new UserDatabaseData();
    static string SavePath => Path.Combine(Application.persistentDataPath, "users.json");
    public const int MaxUsers = 10;

    public static void Load()
    {
        if (File.Exists(SavePath))
        {
            var json = File.ReadAllText(SavePath);
            data = JsonUtility.FromJson<UserDatabaseData>(json) ?? new UserDatabaseData();
        }
        else
        {
            data = new UserDatabaseData();
            Save(); // crea archivo vacio
        }

        EnsureDataConsistency();
    }

    public static void Save()
    {
        EnsureDataConsistency();
        var json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
    }

    public static IReadOnlyList<UserRecord> Users => data.users;

    public static string[] GetUsernames()
    {
        var list = new List<string>();
        foreach (var u in data.users)
        {
            if (u?.credentials == null) continue;
            if (string.IsNullOrEmpty(u.credentials.username)) continue;
            list.Add(u.credentials.username);
        }
        return list.ToArray();
    }

    public static bool Exists(string username)
    {
        return FindUser(username) != null;
    }

    public static bool TryCreate(string username, string password, out string error)
        => TryCreate(username, password, UserType.Player, out error);

    public static bool TryCreate(string username, string password, UserType userType, out string error)
    {
        error = null;

        if (string.IsNullOrWhiteSpace(username)) { error = "Escribe un usuario."; return false; }
        if (string.IsNullOrWhiteSpace(password)) { error = "Escribe una contrasena."; return false; }
        username = username.Trim();
        if (username.Length < 3) { error = "El usuario debe tener al menos 3 caracteres."; return false; }
        if (password.Length < 4) { error = "La contrasena debe tener al menos 4 caracteres."; return false; }

        if (data.users.Count >= MaxUsers) { error = "Limite de 10 usuarios."; return false; }
        if (Exists(username)) { error = "Ese usuario ya existe."; return false; }

        var rec = new UserRecord
        {
            userType = userType,
            credentials = new UserCredentials
            {
                username = username,
                passwordHash = Hash(password)
            }
        };
        rec.EnsureConsistency();
        data.users.Add(rec);
        Debug.Log($"[UserDatabase] Usuario '{username}' creado como {userType}.");
        Save();
        return true;
    }

    public static bool TryValidate(string username, string password, out string error)
        => ValidatePlayerCredentials(username, password, out error);

    public static bool ValidatePlayerCredentials(string username, string password, out string error)
        => ValidateCredentials(username, password, UserType.Player, out error);

    public static bool ValidateTherapistCredentials(string username, string password, out string error)
        => ValidateCredentials(username, password, UserType.Therapist, out error);

    static bool ValidateCredentials(string username, string password, UserType expectedType, out string error)
    {
        error = null;
        username = username?.Trim();
        var rec = FindUser(username);
        if (rec == null)
        {
            error = "Usuario no encontrado.";
            Debug.LogWarning($"[UserDatabase] Fallo login de {expectedType}: usuario '{username}' no existe.");
            return false;
        }

        if (rec.userType != expectedType)
        {
            error = expectedType == UserType.Player ? "El usuario no es un jugador." : "El usuario no es un terapeuta.";
            Debug.LogWarning($"[UserDatabase] Fallo login de {expectedType}: tipo almacenado es {rec.userType}.");
            return false;
        }

        var ok = Verify(password, rec.credentials?.passwordHash);
        if (!ok)
        {
            error = "Contrasena incorrecta.";
            Debug.LogWarning($"[UserDatabase] Contrasena incorrecta para usuario '{username}'.");
            return false;
        }

        Debug.Log($"[UserDatabase] Login exitoso para {expectedType} '{username}'.");
        return true;
    }

    static string Hash(string input)
    {
        using (var sha = SHA256.Create())
        {
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
        }
    }

    static bool Verify(string input, string hash)
    {
        if (string.IsNullOrEmpty(hash) || input == null) return false;
        return Hash(input) == hash;
    }

    static UserRecord FindUser(string username)
    {
        if (string.IsNullOrWhiteSpace(username)) return null;
        username = username.Trim();
        return data.users.Find(u => u != null &&
            u.credentials != null &&
            string.Equals(u.credentials.username, username, StringComparison.OrdinalIgnoreCase));
    }

    static void EnsureDataConsistency()
    {
        if (data == null) data = new UserDatabaseData();
        if (data.users == null) data.users = new List<UserRecord>();

        foreach (var user in data.users)
        {
            user?.EnsureConsistency();
        }
    }
}
