"""Gera sprites, capa e audio do Neon Dash (tudo procedural, sem assets de terceiros).

Uso: python3 tools/gerar_assets.py   (na raiz do repositorio)
"""
import math
import os
import random
import wave

import numpy as np
from PIL import Image, ImageDraw, ImageFilter, ImageFont

RAIZ = os.path.join(os.path.dirname(os.path.abspath(__file__)), "..")
SPRITES = os.path.join(RAIZ, "NeonDash", "Assets", "Resources", "Sprites")
AUDIO = os.path.join(RAIZ, "NeonDash", "Assets", "Resources", "Audio")
os.makedirs(SPRITES, exist_ok=True)
os.makedirs(AUDIO, exist_ok=True)

SS = 4  # supersampling para suavizar as bordas

CIANO = (25, 230, 255)
AZUL = (61, 91, 255)
AZUL_ESC = (20, 24, 80)
ROSA = (255, 45, 149)
LARANJA = (255, 138, 31)
AMARELO = (255, 225, 74)
ROXO = (177, 74, 255)
FUNDO_TOPO = (10, 6, 36)
FUNDO_BASE = (60, 18, 110)


def novo(w, h):
    return Image.new("RGBA", (w * SS, h * SS), (0, 0, 0, 0))


def salvar(img, nome, w, h):
    img = img.resize((w, h), Image.LANCZOS)
    img.save(os.path.join(SPRITES, nome))


def brilho(img, raio, intensidade=1.0):
    """Soma um glow (blur) por tras da imagem."""
    glow = img.filter(ImageFilter.GaussianBlur(raio * SS))
    if intensidade != 1.0:
        r, g, b, a = glow.split()
        a = a.point(lambda v: min(255, int(v * intensidade)))
        glow = Image.merge("RGBA", (r, g, b, a))
    return Image.alpha_composite(glow, img)


def p(v):
    return int(v * SS)


def sprite_player():
    w = 64
    img = novo(w, w)
    d = ImageDraw.Draw(img)
    d.rounded_rectangle([p(3), p(3), p(61), p(61)], radius=p(6), fill=CIANO, outline=(255, 255, 255), width=p(3))
    d.rounded_rectangle([p(10), p(10), p(54), p(54)], radius=p(3), fill=(15, 150, 200))
    # olhos e boca
    d.rectangle([p(16), p(20), p(28), p(32)], fill=(255, 255, 255))
    d.rectangle([p(36), p(20), p(48), p(32)], fill=(255, 255, 255))
    d.rectangle([p(21), p(23), p(28), p(32)], fill=(10, 10, 40))
    d.rectangle([p(41), p(23), p(48), p(32)], fill=(10, 10, 40))
    d.rectangle([p(18), p(41), p(46), p(46)], fill=(10, 10, 40))
    salvar(img, "player.png", w, w)


def sprite_bloco():
    w = 64
    img = novo(w, w)
    d = ImageDraw.Draw(img)
    d.rectangle([0, 0, p(64), p(64)], fill=AZUL_ESC)
    d.rectangle([p(2), p(2), p(62), p(62)], outline=AZUL, width=p(3))
    d.rectangle([p(12), p(12), p(52), p(52)], outline=(35, 55, 170), width=p(2))
    d.line([p(2), p(2), p(12), p(12)], fill=(35, 55, 170), width=p(2))
    d.line([p(62), p(2), p(52), p(12)], fill=(35, 55, 170), width=p(2))
    d.line([p(2), p(62), p(12), p(52)], fill=(35, 55, 170), width=p(2))
    d.line([p(62), p(62), p(52), p(52)], fill=(35, 55, 170), width=p(2))
    salvar(img, "block.png", w, w)


def sprite_chao():
    w = 64
    img = novo(w, w)
    d = ImageDraw.Draw(img)
    d.rectangle([0, 0, p(64), p(64)], fill=(14, 12, 50))
    d.rectangle([0, 0, p(64), p(7)], fill=CIANO)
    d.rectangle([0, p(7), p(64), p(11)], fill=(20, 130, 200))
    d.line([p(32), p(14), p(32), p(64)], fill=(35, 30, 100), width=p(2))
    d.line([0, p(36), p(64), p(36)], fill=(35, 30, 100), width=p(2))
    salvar(img, "ground.png", w, w)


def sprite_espinho():
    w = 64
    img = novo(w, w)
    d = ImageDraw.Draw(img)
    d.polygon([(p(32), p(4)), (p(60), p(60)), (p(4), p(60))], fill=(60, 8, 50), outline=ROSA)
    d.line([(p(32), p(4)), (p(60), p(60)), (p(4), p(60)), (p(32), p(4))], fill=ROSA, width=p(5), joint="curve")
    d.polygon([(p(32), p(22)), (p(46), p(52)), (p(18), p(52))], fill=(130, 15, 90))
    img = brilho(img, 2.5, 0.8)
    salvar(img, "spike.png", w, w)


def sprite_serra():
    w = 128
    img = novo(w, w)
    d = ImageDraw.Draw(img)
    c = 64
    dentes = 12
    pts = []
    for i in range(dentes * 2):
        ang = math.pi * i / dentes
        raio = 60 if i % 2 == 0 else 42
        pts.append((p(c + raio * math.cos(ang)), p(c + raio * math.sin(ang))))
    d.polygon(pts, fill=LARANJA, outline=(255, 255, 255))
    d.ellipse([p(c - 34), p(c - 34), p(c + 34), p(c + 34)], fill=(120, 50, 10))
    d.ellipse([p(c - 22), p(c - 22), p(c + 22), p(c + 22)], fill=LARANJA)
    d.ellipse([p(c - 9), p(c - 9), p(c + 9), p(c + 9)], fill=(40, 12, 4))
    for i in range(3):
        ang = 2 * math.pi * i / 3
        d.line([p(c), p(c), p(c + 30 * math.cos(ang)), p(c + 30 * math.sin(ang))], fill=(40, 12, 4), width=p(4))
    salvar(img, "saw.png", w, w)


def sprite_orbe():
    w = 64
    img = novo(w, w)
    d = ImageDraw.Draw(img)
    d.ellipse([p(10), p(10), p(54), p(54)], fill=AMARELO, outline=(255, 255, 255), width=p(3))
    d.ellipse([p(20), p(20), p(44), p(44)], fill=(255, 190, 40))
    d.ellipse([p(25), p(22), p(33), p(30)], fill=(255, 255, 230))
    img = brilho(img, 4, 1.0)
    salvar(img, "orb.png", w, w)


def sprite_pad():
    w, h = 64, 32
    img = novo(w, h)
    d = ImageDraw.Draw(img)
    d.pieslice([p(4), p(8), p(60), p(56)], 180, 360, fill=AMARELO, outline=(255, 255, 255), width=p(2))
    d.pieslice([p(16), p(16), p(48), p(48)], 180, 360, fill=ROSA)
    d.rectangle([p(0), p(30), p(64), p(32)], fill=AMARELO)
    img = brilho(img, 2, 0.8)
    salvar(img, "pad.png", w, h)


def sprite_seeker():
    w = 64
    img = novo(w, w)
    d = ImageDraw.Draw(img)
    d.polygon([(p(32), p(2)), (p(62), p(32)), (p(32), p(62)), (p(2), p(32))], fill=(50, 10, 90), outline=ROXO)
    d.line([(p(32), p(2)), (p(62), p(32)), (p(32), p(62)), (p(2), p(32)), (p(32), p(2))], fill=ROXO, width=p(4), joint="curve")
    d.ellipse([p(18), p(18), p(46), p(46)], fill=(255, 255, 255))
    d.ellipse([p(25), p(24), p(41), p(40)], fill=ROSA)
    d.ellipse([p(30), p(28), p(36), p(36)], fill=(20, 0, 30))
    img = brilho(img, 3, 0.9)
    salvar(img, "seeker.png", w, w)


def sprite_portal(nome, cor_a, cor_b):
    w, h = 64, 192
    img = novo(w, h)
    d = ImageDraw.Draw(img)
    d.ellipse([p(8), p(4), p(56), p(188)], outline=cor_a, width=p(8))
    d.ellipse([p(18), p(24), p(46), p(168)], outline=cor_b, width=p(5))
    img = brilho(img, 4, 1.0)
    salvar(img, nome, w, h)


def sprite_bandeira():
    w, h = 64, 128
    img = novo(w, h)
    d = ImageDraw.Draw(img)
    d.rectangle([p(28), p(0), p(34), p(128)], fill=(220, 220, 255))
    d.polygon([(p(34), p(6)), (p(62), p(24)), (p(34), p(44))], fill=AMARELO, outline=(255, 255, 255))
    img = brilho(img, 3, 0.8)
    salvar(img, "checkpoint.png", w, h)


def sprite_coracao():
    w = 64
    img = novo(w, w)
    d = ImageDraw.Draw(img)
    d.ellipse([p(6), p(10), p(34), p(38)], fill=ROSA)
    d.ellipse([p(30), p(10), p(58), p(38)], fill=ROSA)
    d.polygon([(p(8), p(30)), (p(56), p(30)), (p(32), p(58))], fill=ROSA)
    d.ellipse([p(14), p(16), p(22), p(24)], fill=(255, 180, 220))
    img = brilho(img, 2, 0.7)
    salvar(img, "heart.png", w, w)


def sprite_quadrado_branco():
    img = Image.new("RGBA", (4, 4), (255, 255, 255, 255))
    img.save(os.path.join(SPRITES, "pixel.png"))


def sprite_fundo():
    """Fundo que repete na horizontal: gradiente + grade + silhuetas de predios neon."""
    w, h = 1024, 576
    rnd = random.Random(7)
    arr = np.zeros((h, w, 3), dtype=np.float32)
    for y in range(h):
        t = y / (h - 1)
        arr[y, :, :] = np.array(FUNDO_TOPO) * (1 - t) + np.array(FUNDO_BASE) * t
    img = Image.fromarray(arr.astype(np.uint8), "RGB").convert("RGBA")
    d = ImageDraw.Draw(img, "RGBA")
    # predios (periodico: termina no mesmo ponto que comeca)
    x = 0
    while x < w:
        largura = rnd.choice([48, 64, 80, 96, 112])
        largura = min(largura, w - x)
        altura = rnd.randint(110, 330)
        d.rectangle([x, h - altura, x + largura - 3, h], fill=(26, 12, 66, 255), outline=(90, 40, 180, 255))
        for jy in range(h - altura + 14, h - 10, 26):
            for jx in range(x + 8, x + largura - 14, 18):
                if rnd.random() < 0.35:
                    d.rectangle([jx, jy, jx + 6, jy + 9], fill=(255, 225, 74, 160))
        x += largura
    # grade neon
    for gx in range(0, w, 64):
        d.line([gx, 0, gx, h], fill=(80, 60, 200, 28), width=1)
    for gy in range(0, h, 64):
        d.line([0, gy, w, gy], fill=(80, 60, 200, 28), width=1)
    img.convert("RGB").save(os.path.join(SPRITES, "background.png"))


def fonte(tamanho):
    for caminho in (
        "/usr/share/fonts/truetype/liberation2/LiberationSans-Bold.ttf",
        "/usr/share/fonts/truetype/dejavu/DejaVuSans-Bold.ttf",
    ):
        if os.path.exists(caminho):
            return ImageFont.truetype(caminho, tamanho)
    return ImageFont.load_default()


def texto_neon(img, xy, txt, tamanho, cor, ancora="mm"):
    camada = Image.new("RGBA", img.size, (0, 0, 0, 0))
    d = ImageDraw.Draw(camada)
    f = fonte(tamanho)
    d.text(xy, txt, font=f, fill=cor, anchor=ancora)
    glow = camada.filter(ImageFilter.GaussianBlur(tamanho * 0.12))
    img.alpha_composite(glow)
    img.alpha_composite(glow)
    img.alpha_composite(camada)
    d2 = ImageDraw.Draw(img)
    d2.text(xy, txt, font=f, fill=(255, 255, 255), anchor=ancora, stroke_width=0)


def capa(largura, altura, nome, so_arte=False):
    """Capa para o itch.io (630x500) e fundo do menu (1920x1080)."""
    rnd = random.Random(11)
    arr = np.zeros((altura, largura, 3), dtype=np.float32)
    for y in range(altura):
        t = y / (altura - 1)
        arr[y, :, :] = np.array(FUNDO_TOPO) * (1 - t) + np.array((120, 30, 140)) * t
    img = Image.fromarray(arr.astype(np.uint8), "RGB").convert("RGBA")
    d = ImageDraw.Draw(img, "RGBA")
    chao = int(altura * 0.78)
    # sol / lua neon
    cx, cy, r = largura * 0.72, chao - altura * 0.12, altura * 0.26
    for i in range(int(r), 0, -2):
        t = i / r
        cor = (int(255), int(60 + 160 * (1 - t)), int(120 + 40 * t), 255)
        d.ellipse([cx - i, cy - i, cx + i, cy + i], fill=cor)
    for k in range(7):
        y0 = cy + k * r / 7 * 0.9 - r * 0.1
        d.rectangle([cx - r, y0, cx + r, y0 + (k + 1) * 1.6], fill=(70, 20, 110, 255))
    # predios
    x = 0
    while x < largura:
        wb = rnd.randint(int(largura * 0.04), int(largura * 0.09))
        hb = rnd.randint(int(altura * 0.12), int(altura * 0.42))
        d.rectangle([x, chao - hb, x + wb - 4, chao], fill=(22, 10, 56, 255), outline=(110, 50, 200, 255))
        x += wb
    # chao
    d.rectangle([0, chao, largura, altura], fill=(12, 10, 44, 255))
    d.rectangle([0, chao, largura, chao + max(4, altura // 90)], fill=CIANO)
    for i in range(-10, 40):
        d.line([largura / 2 + (i - 15) * largura * 0.02, chao + 6, largura / 2 + (i - 15) * largura * 0.12, altura], fill=(80, 60, 200, 120), width=2)
    # espinhos, bloco e jogador
    u = altura * 0.11
    sp = Image.open(os.path.join(SPRITES, "spike.png")).convert("RGBA")
    pl = Image.open(os.path.join(SPRITES, "player.png")).convert("RGBA")
    bl = Image.open(os.path.join(SPRITES, "block.png")).convert("RGBA")
    orb = Image.open(os.path.join(SPRITES, "orb.png")).convert("RGBA")
    saw = Image.open(os.path.join(SPRITES, "saw.png")).convert("RGBA")

    def cola(spr, x, y, tam, rot=0):
        s = spr.resize((int(tam), int(tam * spr.height / spr.width)), Image.LANCZOS)
        if rot:
            s = s.rotate(rot, expand=True, resample=Image.BICUBIC)
        img.alpha_composite(s, (int(x - s.width / 2), int(y - s.height / 2)))

    cola(sp, largura * 0.30, chao - u * 0.5, u)
    cola(sp, largura * 0.30 + u, chao - u * 0.5, u)
    cola(bl, largura * 0.52, chao - u * 0.5, u)
    cola(bl, largura * 0.52 + u, chao - u * 0.5, u)
    cola(bl, largura * 0.52 + u, chao - u * 1.5, u)
    cola(saw, largura * 0.88, chao - u * 2.6, u * 1.6, 20)
    cola(orb, largura * 0.43, chao - u * 3.3, u * 0.9)
    cola(pl, largura * 0.37, chao - u * 2.6, u * 1.1, -22)
    # rastro do jogador
    for i in range(1, 8):
        cola(pl, largura * 0.37 - i * u * 0.55, chao - u * 2.6 + i * u * 0.12, u * 1.1 * (1 - i * 0.1), -22 - i * 6)
        pass
    # titulo
    if not so_arte:
        texto_neon(img, (largura / 2, altura * 0.18), "NEON DASH", int(altura * 0.19), ROSA)
        texto_neon(img, (largura / 2, altura * 0.31), "PULE. DESVIE. CHEGUE AO PORTAL.", int(altura * 0.04), CIANO)
    img.convert("RGB").save(os.path.join(SPRITES if so_arte else RAIZ, nome))


# ---------------------------------------------------------------- audio

SR = 22050


def grava(nome, sinal):
    sinal = np.clip(sinal, -1, 1)
    dados = (sinal * 32767 * 0.8).astype(np.int16)
    with wave.open(os.path.join(AUDIO, nome), "wb") as w:
        w.setnchannels(1)
        w.setsampwidth(2)
        w.setframerate(SR)
        w.writeframes(dados.tobytes())


def tempo(dur):
    return np.arange(int(SR * dur)) / SR


def quadrada(freq, t, duty=0.5):
    return np.where((t * freq) % 1.0 < duty, 1.0, -1.0)


def serra_onda(freq, t):
    return 2 * ((t * freq) % 1.0) - 1


def env(n, ataque=0.005, queda=6.0):
    t = np.arange(n) / SR
    e = np.exp(-queda * t)
    a = np.minimum(1.0, t / ataque)
    return e * a


def nota_hz(m):
    return 440.0 * 2 ** ((m - 69) / 12)


def sfx_pulo():
    t = tempo(0.22)
    f = 300 + 700 * (t / 0.22)
    fase = np.cumsum(f) / SR
    s = np.where((fase % 1.0) < 0.5, 1.0, -1.0) * env(len(t), queda=10) * 0.5
    grava("jump.wav", s)


def sfx_orbe():
    t1, t2 = tempo(0.07), tempo(0.25)
    a = quadrada(988, t1) * env(len(t1), queda=5) * 0.4
    b = quadrada(1319, t2) * env(len(t2), queda=9) * 0.4
    grava("coin.wav", np.concatenate([a, b]))


def sfx_morte():
    t = tempo(0.5)
    f = 400 * np.exp(-3.0 * t) + 40
    fase = np.cumsum(f) / SR
    ruido = np.random.RandomState(1).uniform(-1, 1, len(t))
    s = (np.where((fase % 1.0) < 0.5, 1.0, -1.0) * 0.5 + ruido * 0.5) * env(len(t), queda=5)
    grava("hit.wav", s * 0.9)


def sfx_pad():
    t = tempo(0.3)
    f = 200 + 1200 * (t / 0.3) ** 0.7
    fase = np.cumsum(f) / SR
    s = np.sin(2 * np.pi * fase) * env(len(t), queda=6)
    grava("pad.wav", s * 0.7)


def sfx_checkpoint():
    partes = []
    for m in (72, 76, 79):
        tt = tempo(0.09)
        partes.append(quadrada(nota_hz(m), tt, 0.25) * env(len(tt), queda=4) * 0.35)
    grava("checkpoint.wav", np.concatenate(partes))


def sfx_vitoria():
    partes = []
    for m, d in ((72, 0.12), (76, 0.12), (79, 0.12), (84, 0.45)):
        tt = tempo(d)
        partes.append((quadrada(nota_hz(m), tt, 0.25) * 0.3 + quadrada(nota_hz(m) * 2, tt) * 0.1) * env(len(tt), queda=3))
    grava("win.wav", np.concatenate(partes))


def sfx_derrota():
    partes = []
    for m, d in ((67, 0.2), (63, 0.2), (60, 0.2), (55, 0.6)):
        tt = tempo(d)
        partes.append(serra_onda(nota_hz(m), tt) * 0.35 * env(len(tt), queda=3))
    grava("lose.wav", np.concatenate(partes))


def sfx_clique():
    t = tempo(0.08)
    grava("click.wav", quadrada(660, t) * env(len(t), queda=25) * 0.4)


def sfx_tempo():
    t = tempo(0.1)
    grava("tick.wav", quadrada(880, t, 0.25) * env(len(t), queda=18) * 0.35)


def musica(nome, bpm, progressao, arp_oitava, intensa, compassos=16):
    """Loop chiptune: kick + hihat + baixo + arpejo + lead opcional."""
    passo = 60.0 / bpm / 4  # semicolcheia
    total_passos = compassos * 16
    n = int(total_passos * passo * SR)
    saida = np.zeros(n, dtype=np.float32)
    rnd = np.random.RandomState(3)

    def mixa(inicio, sinal):
        i = int(inicio * SR)
        fim = min(n, i + len(sinal))
        if fim > i:
            saida[i:fim] += sinal[: fim - i]

    for c in range(compassos):
        raiz, tipo = progressao[c % len(progressao)]
        acorde = [0, 3, 7, 12] if tipo == "m" else [0, 4, 7, 12]
        for s in range(16):
            idx = c * 16 + s
            tempo_s = idx * passo
            # kick nos tempos fortes
            if s % 4 == 0 and (intensa or s % 8 == 0):
                tt = tempo(0.15)
                f = 120 * np.exp(-25 * tt) + 45
                fase = np.cumsum(f) / SR
                mixa(tempo_s, np.sin(2 * np.pi * fase) * np.exp(-14 * tt) * 0.9)
            # hihat nos contratempos
            if s % 2 == 1 and intensa:
                tt = tempo(0.04)
                mixa(tempo_s, rnd.uniform(-1, 1, len(tt)) * np.exp(-90 * tt) * 0.18)
            # snare
            if s % 8 == 4:
                tt = tempo(0.12)
                mixa(tempo_s, rnd.uniform(-1, 1, len(tt)) * np.exp(-28 * tt) * 0.3)
            # baixo em colcheias
            if s % 2 == 0:
                tt = tempo(passo * 2 * 0.9)
                f = nota_hz(raiz - 24 + (12 if (s % 8 == 6) else 0))
                mixa(tempo_s, quadrada(f, tt, 0.5) * env(len(tt), queda=6) * 0.30)
            # arpejo
            tt = tempo(passo * 0.95)
            f = nota_hz(raiz + arp_oitava + acorde[s % 4])
            mixa(tempo_s, quadrada(f, tt, 0.25) * env(len(tt), queda=9) * (0.14 if intensa else 0.10))
            # lead a cada 2 compassos
            if intensa and c % 4 >= 2 and s in (0, 3, 6, 8, 11, 14):
                tt = tempo(passo * 2.2)
                f = nota_hz(raiz + 24 + acorde[(s // 3) % 4])
                mixa(tempo_s, (quadrada(f, tt, 0.5) * 0.5 + serra_onda(f, tt) * 0.2) * env(len(tt), queda=4) * 0.16)
    pico = np.max(np.abs(saida))
    grava(nome, saida / pico * 0.9)


def gerar():
    sprite_player()
    sprite_bloco()
    sprite_chao()
    sprite_espinho()
    sprite_serra()
    sprite_orbe()
    sprite_pad()
    sprite_seeker()
    sprite_portal("finish.png", AMARELO, ROSA)
    sprite_bandeira()
    sprite_coracao()
    sprite_quadrado_branco()
    sprite_fundo()
    capa(630, 500, "capa_itch.png")
    capa(1920, 1080, "menu_bg.png", so_arte=True)

    sfx_pulo()
    sfx_orbe()
    sfx_morte()
    sfx_pad()
    sfx_checkpoint()
    sfx_vitoria()
    sfx_derrota()
    sfx_clique()
    sfx_tempo()
    # progressao (nota raiz midi, tipo do acorde) - Am F C G / Am F Dm E
    prog_jogo = [(57, "m"), (53, "M"), (60, "M"), (55, "M"), (57, "m"), (53, "M"), (50, "m"), (52, "M")]
    musica("music.wav", 150, prog_jogo, 12, True, compassos=24)


if __name__ == "__main__":
    gerar()
    print("ok")
