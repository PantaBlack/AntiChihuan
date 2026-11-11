package pe.antichihuan.dto;

public class AuthDtos {

    public static class LoginRequest {
        public String email;
        public String password;
    }

    public static class RegisterRequest {
        public String nombreCompleto;
        public String email;
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
