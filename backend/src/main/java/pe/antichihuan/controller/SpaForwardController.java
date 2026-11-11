package pe.antichihuan.controller;

import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.RequestMapping;

@Controller
public class SpaForwardController {

    @RequestMapping(value = {
        "/login",
        "/register",
        "/dashboard",
        "/transacciones",
        "/presupuestos",
        "/categorias",
        "/ia"
    })
    public String forwardToIndex() {
        return "forward:/index.html";
    }
}
