export function setTheme(theme) {
    const doc = document.firstElementChild
    doc.setAttribute('color-scheme', theme)
}