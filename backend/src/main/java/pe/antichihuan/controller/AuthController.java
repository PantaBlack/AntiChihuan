package pe.antichihuan.controller;

import jakarta.validation.Valid;
import org.springframework.validation.BindingResult;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.web.bind.annotation.*;
import pe.antichihuan.config.JwtUtil;
import pe.antichihuan.dto.AuthDtos.AuthResponse;
import pe.antichihuan.dto.AuthDtos.LoginRequest;
import pe.antichihuan.dto.AuthDtos.RegisterRequest;
import pe.antichihuan.model.Usuario;
import pe.antichihuan.repository.UsuarioRepository;

import java.util.Map;

@RestController
@RequestMapping("/api/auth")
public class AuthController {

    private final UsuarioRepository usuarioRepository;
    private final PasswordEncoder passwordEncoder;
    private final JwtUtil jwtUtil;

    public AuthController(UsuarioRepository usuarioRepository, PasswordEncoder passwordEncoder, JwtUtil jwtUtil) {
        this.usuarioRepository = usuarioRepository;
        this.passwordEncoder = passwordEncoder;
        this.jwtUtil = jwtUtil;
    }

    @PostMapping("/login")
    public ResponseEntity<?> login(@Valid @RequestBody LoginRequest request, BindingResult result) {
        if (result.hasErrors()) {
            String errorMsg = result.getAllErrors().get(0).getDefaultMessage();
            return ResponseEntity.status(HttpStatus.UNAUTHORIZED).body(Map.of("message", errorMsg));
        }

        var userOpt = usuarioRepository.findByEmailIgnoreCase(request.email.trim());
        if (userOpt.isEmpty() || !passwordEncoder.matches(request.password, userOpt.get().getPasswordHash())) {
            return ResponseEntity.status(HttpStatus.UNAUTHORIZED)
                    .body(Map.of("message", "Credenciales incorrectas o usuario no registrado."));
        }

        Usuario u = userOpt.get();
        String token = jwtUtil.generateToken(u.getId(), u.getEmail(), u.getNombreCompleto());
        return ResponseEntity.ok(new AuthResponse(token, u.getId(), u.getNombreCompleto(), u.getEmail()));
    }

    @PostMapping("/register")
    public ResponseEntity<?> register(@Valid @RequestBody RegisterRequest request, BindingResult result) {
        if (result.hasErrors()) {
            String errorMsg = result.getAllErrors().get(0).getDefaultMessage();
            return ResponseEntity.badRequest().body(Map.of("message", errorMsg));
        }

        String emailLimpio = request.email.trim().toLowerCase();
        if (usuarioRepository.existsByEmailIgnoreCase(emailLimpio)) {
            return ResponseEntity.badRequest()
                    .body(Map.of("message", "Ya existe una cuenta registrada con este correo electrónico."));
        }

        Usuario nuevo = new Usuario(
                request.nombreCompleto.trim(),
                emailLimpio,
                passwordEncoder.encode(request.password)
        );
        usuarioRepository.save(nuevo);

        String token = jwtUtil.generateToken(nuevo.getId(), nuevo.getEmail(), nuevo.getNombreCompleto());
        return ResponseEntity.status(HttpStatus.CREATED)
                .body(new AuthResponse(token, nuevo.getId(), nuevo.getNombreCompleto(), nuevo.getEmail()));
    }

    @GetMapping("/usuarios")
    public ResponseEntity<?> listarUsuarios() {
        var usuarios = usuarioRepository.findAll().stream().map(u -> Map.of(
                "id", u.getId(),
                "nombreCompleto", u.getNombreCompleto(),
                "email", u.getEmail(),
                "fechaRegistro", u.getFechaRegistro() != null ? u.getFechaRegistro().toString() : ""
        )).toList();
        return ResponseEntity.ok(usuarios);
    }
}
