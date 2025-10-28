using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UserUIController : MonoBehaviour
{
    [Header("Login")]
    public GameObject panelLogin;
    public TMP_Dropdown dropdownUsuarios;
    public TMP_InputField inputPassword;
    public TMP_Text txtFeedback;

    [Header("Crear Usuario")]
    public GameObject panelCrear;
    public TMP_InputField inputNuevoUsuario;
    public TMP_InputField inputNuevaPass;
    public TMP_InputField inputRepitePass;
    public TMP_Text txtFeedbackCrear;

    void Start()
    {
        // 🔹 Carga los usuarios guardados
        UserDatabase.Load();

        // 🔹 Configura visibilidad inicial
        panelCrear.SetActive(false);
        panelLogin.SetActive(true);

        // 🔹 Llena el dropdown con los nombres existentes
        RefrescarListaUsuarios();

        // Si no hay usuarios, abre el panel de creación
        if (UserDatabase.Users.Count == 0)
        {
            panelLogin.SetActive(false);
            panelCrear.SetActive(true);
            txtFeedbackCrear.text = "Crea tu primer usuario 🤟";
        }
    }

    void RefrescarListaUsuarios()
    {
        dropdownUsuarios.ClearOptions();
        var names = UserDatabase.GetUsernames();
        if (names.Length == 0) names = new[] { "(sin usuarios)" };
        dropdownUsuarios.AddOptions(new System.Collections.Generic.List<string>(names));
    }

    // ===========================================================
    // 🔹 Botones de LOGIN
    // ===========================================================
    public void OnClick_IniciarSesion()
    {
        if (UserDatabase.Users.Count == 0)
        {
            txtFeedback.text = "Primero crea un usuario.";
            return;
        }

        var username = dropdownUsuarios.options[dropdownUsuarios.value].text;
        if (username == "(sin usuarios)")
        {
            txtFeedback.text = "No hay usuarios. Crea uno.";
            return;
        }

        var pass = inputPassword.text;
        if (UserDatabase.TryValidate(username, pass, out string error))
        {
            txtFeedback.text = "¡Listo! ✅";

            // 🔹 Asegura que exista la sesión global
            if (GameSession.Instance == null)
            {
                var go = new GameObject("_GameSession");
                go.AddComponent<GameSession>();
            }

            GameSession.Instance.SetUsuarioActual(username);

            // 🔹 Cambia a la escena de selección
            SceneManager.LoadScene("SeleccionCarroPista");
        }
        else
        {
            txtFeedback.text = error; // "Contraseña incorrecta" o "Usuario no encontrado"
        }
    }

    public void OnClick_IrANuevoUsuario()
{
    // 🔹 Asegúrate de que el panel de creación se reactive correctamente
    if (panelCrear == null)
    {
        Debug.LogError("panelCrear no está asignado en el Inspector.");
        return;
    }

    panelLogin.SetActive(false);
    panelCrear.SetActive(true);

    // 🔹 Reactiva todos los campos del panel de creación
    foreach (Transform child in panelCrear.transform)
    {
        child.gameObject.SetActive(true);
    }

    txtFeedback.text = "";
    txtFeedbackCrear.text = "";
    inputNuevoUsuario.text = "";
    inputNuevaPass.text = "";
    inputRepitePass.text = "";

    Debug.Log("🟢 Panel de creación reactivado correctamente.");
}


    // ===========================================================
    // 🔹 Botones de CREAR USUARIO
    // ===========================================================
    public void OnClick_CrearUsuario()
    {
        var u = inputNuevoUsuario.text.Trim();
        var p1 = inputNuevaPass.text;
        var p2 = inputRepitePass.text;

        if (p1 != p2)
        {
            txtFeedbackCrear.text = "Las contraseñas no coinciden.";
            return;
        }

        if (UserDatabase.TryCreate(u, p1, out string error))
        {
            txtFeedbackCrear.text = "Usuario creado ✅";

            // 🔹 Volver al login
            panelCrear.SetActive(false);
            panelLogin.SetActive(true);
            RefrescarListaUsuarios();
        }
        else
        {
            txtFeedbackCrear.text = error;
        }
    }

    public void OnClick_CancelarCrear()
    {
        panelCrear.SetActive(false);
        panelLogin.SetActive(true);
        txtFeedbackCrear.text = "";
    }
}
