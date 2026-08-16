package com.gchunyan.waterpipe.ui

import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.compose.foundation.isSystemInDarkTheme
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Surface
import androidx.compose.material3.darkColorScheme
import androidx.compose.material3.lightColorScheme
import androidx.compose.runtime.Composable
import androidx.compose.ui.graphics.Color
import androidx.core.view.WindowCompat

class MainActivity : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        WindowCompat.setDecorFitsSystemWindows(window, false)
        setContent {
            WaterPipeTheme {
                Surface(color = MaterialTheme.colorScheme.background) {
                    WaterPipeApp()
                }
            }
        }
    }
}

@Composable
fun WaterPipeTheme(content: @Composable () -> Unit) {
    val colors = if (isSystemInDarkTheme()) {
        darkColorScheme(
            primary = Color(0xFF90CAF9),
            secondary = Color(0xFFFFCC80),
            background = Color(0xFF0D1B2A),
            surface = Color(0xFF1B263B)
        )
    } else {
        lightColorScheme(
            primary = Color(0xFF1976D2),
            secondary = Color(0xFFFF6F00),
            background = Color(0xFFF0F8FF),
            surface = Color(0xFFFFFFFF)
        )
    }
    MaterialTheme(colorScheme = colors, content = content)
}
