# Symbol Door - Design Refactor #4

## Visual Philosophy

**Mystical Puzzle Aesthetic** inspired by Chants of Sennaar
- Dark backgrounds with golden accents
- Clear visual hierarchy and feedback
- Smooth, purposeful animations (no jarring transitions)
- Accessible color contrast

## Color Palette

```
Dark Background:    #141E26 (Deep purple-black)
Accent Gold:        #EBC733 (Warm, luxurious)
Accent Cyan:        #33CCFF (Mystical, cool)
Error Red:          #F24C4C (Bright, urgent)
Success Green:      #4CE651 (Emerald, affirming)
Text/Symbol:        #F2F2F2 (Light gray)
Border Gold:        #D9B327 (Darker gold for contrast)
```

## UI Layout

```
┌─────────────────────────────────────────────────────────┐
│  SYMBOL DOOR PUZZLE                          [CLOSE]    │
├──────────────┬──────────────────────┬──────────────────┤
│              │                      │                  │
│  ☀️           │      🚪 DOOR 🚪     │     день       │
│              │   [with glow]        │                  │
│  🌙   ──────→│                      │     ночь        │
│              │                      │                  │
│  🕐           │                      │     время       │
│              │                      │                  │
├──────────────┴──────────────────────┴──────────────────┤
│  [ПРОВЕРИТЬ]                           [СБРОС]        │
└─────────────────────────────────────────────────────────┘
```

## Button Styling

**Symbol Buttons (90x90):**
- Background: Dark
- Border: 2px gold outline
- Text: Large emoji (48px) + semi-transparent glow
- Hover: Smooth fade to gold (150ms ease-out)
- Selected: Gold highlight until connection made

**Meaning Buttons (110x70):**
- Background: Dark
- Border: 1px gold outline
- Text: Russian text (32px)
- Connected: Cyan highlight
- Hover: Subtle glow

**Door (Variable):**
- Background: Dark with 4px gold border
- State: Pulsing glow on success (animation)
- Error: Brief red flash (4x flicker)

**Action Buttons:**
- Check: Gold background, black text
- Reset: Cyan background, black text
- Both: Bold, uppercase Cyrillic text

## Animations

### Button Selection
- Duration: 200ms
- Easing: EaseOutQuad
- Effect: Dark → Gold fade

### Connection Line
- Duration: 300ms fade-in
- Effect: LineRenderer with alpha animation
- Exit: 200ms fade-out

### Success Feedback
- Sound: Subtle ding (optional)
- Visual: Door pulses, transitions to green tint
- Duration: 400ms smooth pulse

### Error Feedback
- Sound: Error buzz (optional)
- Visual: 4x red flash with 150ms each pulse
- Effect: Door and buttons flicker
- Recovery: Auto-reset available

### Button Error Flash
- On incomplete submit: Check button flashes red 3x
- Duration: 150ms per flash
- Message: "[symbols not matched] 选择所有 pairs"

## Interaction Flow

1. **User clicks symbol** → Gold highlight, stays selected
2. **User clicks meaning** → Cyan highlight on meaning, gold line animates between
3. **Repeat until all connected**
4. **Click Check** → Success or Error animation
5. **On Success** → Door glows, auto-closes after 1s
6. **On Error** → Flashing, Reset button enabled
7. **User clicks Reset** → Clear all connections, deselect symbol

## Font & Typography

- **Symbols**: Large emoji (48px) - universal, no translation needed
- **Meanings**: Cyrillic text (32px) - bold, readable
- **Labels**: Arial, 16px, uppercase, slight letter-spacing

## Accessibility

✓ High contrast (gold on dark, cyan on dark)
✓ Large touch targets (90x90 minimum)
✓ Clear visual feedback for all states
✓ No animations that disable on-demand
✓ Color-blind safe palette (includes cyan + gold + red combo)

## Performance Optimization

- Coroutines properly stopped to prevent memory leaks
- LineRenderer only created when needed
- Colors cached in static `SymbolDoorUIDesign` class
- Easing functions inlined (no GC allocation)

## Future Enhancements

1. **Particle Effects**: Burst on success
2. **Sound Design**: Subtle mystical audio cues
3. **Symbol Variety**: Different emoji sets per level
4. **Difficulty Scaling**: More symbols/meanings = harder
5. **Leaderboard**: Track fastest solve times
