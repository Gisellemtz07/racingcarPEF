using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

[Serializable]
public class UserRecord
{
    public string username;
    public string passwordHash;
}

[Serializable]
class UserDatabaseData
{
    public List<UserRecord> users = new List<UserRecord>();
}

public static class UserDatabase
{
    private static UserDatabaseData data = new UserDatabaseData();
    private static string SavePath => Path.Combine(Application.persistentDataPath, "users.json");
    public const int MaxUsers = 10;

    // ===========================================================
    // 🔹 Cargar / Guardar
    // ===========================================================

    public static void Load()
    {
        try
        {
            if (File.Exists(SavePath))
            {
                var json = File.ReadAllText(SavePath);
                data = JsonUtility.FromJson<UserDatabaseData>(json) ?? new UserDatabaseData();
                Debug.Log($" [UserDatabase] {data.users.Count} usuarios cargados desde JSON.");
            }
            else
            {
                data = new UserDatabaseData();
                Save(); // crea archivo vacío
                Debug.Log("[UserDatabase] No existía users.json, se creó uno nuevo.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($" Error al cargar base de datos: {ex.Message}");
            data = new UserDatabaseData();
        }
    }

    public static void Save()
    {
        try
        {
            var json = JsonUtility.ToJson(data, true);
            File.WriteAllText(SavePath, json);
            Debug.Log($"[UserDatabase] Guardado exitoso ({data.users.Count} usuarios).");
        }
        catch (Exception ex)
        {
            Debug.LogError($" Error al guardar base de datos: {ex.Message}");
        }
    }

    // ===========================================================
    // 🔹 Accesos rápidos
    // ===========================================================

    public static IReadOnlyList<UserRecord> Users => data.users;

    public static string[] GetUsernames()
    {
        var list = new List<string>();
        foreach (var u in data.users) list.Add(u.username);
        return list.ToArray();
    }

    public static bool Exists(string username)
    {
        return data.users.Exists(u =>
            string.Equals(u.username, username, StringComparison.OrdinalIgnoreCase));
    }

    // ===========================================================
    // 🔹 Crear / Validar usuario
    // ===========================================================

    public static bool TryCreate(string username, string password, out string error)
    {
        error = "";

        Load();

        if (data.users.Count >= MaxUsers)
        {
            error = $"Solo se permiten {MaxUsers} usuarios.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(username))
        {
            error = "Escribe un usuario.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            error = "Escribe una contraseña.";
            return false;
        }

        if (username.Length < 3)
        {
            error = "El usuario debe tener al menos 3 caracteres.";
            return false;
        }

        if (password.Length < 4)
        {
            error = "La contraseña debe tener al menos 4 caracteres.";
            return false;
        }

        if (Exists(username))
        {
            error = $"El nombre '{username}' ya existe.";
            return false;
        }

        data.users.Add(new UserRecord
        {
            username = username,
            passwordHash = Hash(password)
        });

        Save();
        Debug.Log($"✅ [UserDatabase] Usuario creado: {username}");
        return true;
    }

    public static bool TryValidate(string username, string password, out string error)
    {
        error = null;
        username = username?.Trim();

        var rec = data.users.Find(u =>
            string.Equals(u.username, username, StringComparison.OrdinalIgnoreCase));

        if (rec == null)
        {
            error = "Usuario no encontrado.";
            Debug.LogWarning($" [UserDatabase] '{username}' no existe.");
            return false;
        }

        var ok = Verify(password, rec.passwordHash);
        if (!ok)
        {
            error = "Contraseña incorrecta.";
            Debug.LogWarning($" [UserDatabase] Contraseña incorrecta para '{username}'.");
            return false;
        }

        Debug.Log($" [UserDatabase] Login exitoso para '{username}'.");
        return true;
    }

    // ===========================================================
    // 🔹 Métodos internos
    // ===========================================================

    private static string Hash(string input)
    {
        using (var sha = SHA256.Create())
        {
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
        }
    }

    private static bool Verify(string input, string hash)
    {
        if (string.IsNullOrEmpty(hash) || input == null) return false;
        return Hash(input) == hash;
    }
}
