package pe.antichihuan.dto;

import jakarta.validation.constraints.Email;
import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.Pattern;
import jakarta.validation.constraints.Size;

public class AuthDtos {

    public static class LoginRequest {
        @NotBlank(message = "El correo electrónico es requerido.")
        @Email(message = "Formato de correo electrónico inválido.")
        public String email;

        @NotBlank(message = "La contraseña es requerida.")
        public String password;
    }

    public static class RegisterRequest {
        @NotBlank(message = "El nombre completo es obligatorio.")
        @Size(min = 3, max = 120, message = "El nombre completo debe tener entre 3 y 120 caracteres.")
        @Pattern(regexp = "^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ\\s]+$", message = "El nombre solo puede contener letras y espacios.")
        public String nombreCompleto;

        @NotBlank(message = "El correo electrónico es obligatorio.")
        @Email(message = "Debe ingresar un formato de correo electrónico válido (ejemplo: usuario@correo.com).")
        @Size(max = 120, message = "El correo electrónico no puede exceder los 120 caracteres.")
        public String email;

        @NotBlank(message = "La contraseña es obligatoria.")
        @Size(min = 6, max = 50, message = "La contraseña debe tener al menos 6 caracteres.")
        @Pattern(regexp = "^(?=.*[A-Za-z])(?=.*\\d).+$", message = "La contraseña debe contener al menos una letra y un número.")
        public String password;
    }

    public static class AuthResponse {
        public String token;
        public Long usuarioId;
        public String nombreCompleto;
        public String email;

        public AuthResponse(String token, Long usuarioId, String nombreCompleto, String email) {
            this.token = token;
            this.usuarioId = usuarioId;
            this.nombreCompleto = nombreCompleto;
            this.email = email;
        }
    }
}
