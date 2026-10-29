# UX and Accessibility

**Status:** Draft
**Last reviewed:** 2026-10-29

## Experience

- **Experience principles:** To be defined
- **Priority user journeys:** To be defined
- **Required screens or states:** To be defined
- **Content and language:** To be defined

## Interface

- **Visual references or design system:** Bootstrap 5 and the confirmed color system below. Any provided image is a reference for chromatic identity only; do not copy its layout, components, or structure.
- **Standard layout:** Header, collapsible navigation menu, content area, and footer
- **Responsive behavior:** Support desktop, tablet, and smartphone viewports. Preserve the standard layout and usable navigation across these device classes.
- **Loading, empty, and error states:** To be defined

### Color system

Use the following named color tokens and usage guidance as the visual baseline.

#### Brand

Violet is the institutional color, used for the brand, navigation, primary buttons, links, active elements, and primary charts.

| Token | Value |
|---|---|
| `--brand-50` | `#F6F2FF` |
| `--brand-100` | `#EEE7FF` |
| `--brand-200` | `#DDD0FF` |
| `--brand-300` | `#C4AEFF` |
| `--brand-400` | `#A587FF` |
| `--brand-500` | `#7857E8` |
| `--brand-600` | `#6544D0` |
| `--brand-700` | `#5335B2` |
| `--brand-800` | `#422A8F` |
| `--brand-900` | `#34206F` |

#### Accent

Use light lilac for secondary surfaces, card backgrounds, selections, hover states, dashboards, and informational panels.

| Token | Value |
|---|---|
| `--accent-50` | `#FBF9FF` |
| `--accent-100` | `#F4F0FF` |
| `--accent-200` | `#ECE4FF` |
| `--accent-300` | `#DDD0FF` |
| `--accent-400` | `#CAB5FF` |

#### Neutrals

Use neutral colors for the predominant surfaces, borders, and text hierarchy.

| Token | Value |
|---|---|
| `--surface` | `#FFFFFF` |
| `--surface-alt` | `#FAFAFC` |
| `--surface-soft` | `#F7F7F9` |
| `--border-light` | `#F0F0F4` |
| `--border-default` | `#E5E7EB` |
| `--border-strong` | `#D5D8E0` |
| `--text-primary` | `#1F2937` |
| `--text-secondary` | `#6B7280` |
| `--text-muted` | `#9CA3AF` |
| `--text-disabled` | `#C7CDD7` |

#### Semantic colors

| Role | Token | Value | Intended use |
|---|---|---|---|
| Success | `--success-50` | `#ECFDF5` | Success surfaces |
| Success | `--success-100` | `#DDF8EE` | Success surfaces |
| Success | `--success-500` | `#3DBE8B` | Completed status, approvals, positive progress, success indicators |
| Success | `--success-600` | `#2FA472` | Stronger success emphasis |
| Warning | `--warning-50` | `#FFF5EC` | Warning surfaces |
| Warning | `--warning-100` | `#FFE7D3` | Warning surfaces |
| Warning | `--warning-500` | `#FF9A57` | Pending tasks, alerts, open tasks |
| Warning | `--warning-600` | `#F07C2E` | Stronger warning emphasis |
| Danger | `--danger-50` | `#FFF0F0` | Error surfaces |
| Danger | `--danger-100` | `#FFD8D8` | Error surfaces |
| Danger | `--danger-500` | `#F56C6C` | Error indication |
| Danger | `--danger-600` | `#E54848` | Stronger error emphasis |
| Information | `--info-50` | `#EEF7FF` | Informational surfaces |
| Information | `--info-100` | `#D8ECFF` | Informational surfaces |
| Information | `--info-500` | `#4D9EFF` | Information indicators |
| Information | `--info-600` | `#2D7FF0` | Stronger information emphasis |

#### Theme roles

The light theme uses:

| Role | Value |
|---|---|
| `--background` | `#F8F9FB` |
| `--surface` | `#FFFFFF` |
| `--primary` | `#7857E8` |
| `--primary-hover` | `#6544D0` |
| `--text-primary` | `#1F2937` |
| `--text-secondary` | `#6B7280` |
| `--success` | `#3DBE8B` |
| `--warning` | `#FF9A57` |
| `--danger` | `#F56C6C` |
| `--info` | `#4D9EFF` |

The dark theme retains the same chromatic identity:

| Role | Value |
|---|---|
| `--background` | `#111827` |
| `--surface` | `#1F2937` |
| `--primary` | `#A587FF` |
| `--primary-hover` | `#C4AEFF` |
| `--text-primary` | `#F9FAFB` |
| `--text-secondary` | `#D1D5DB` |
| `--success` | `#52C79B` |
| `--warning` | `#FFB074` |
| `--danger` | `#FF8B8B` |
| `--info` | `#74B6FF` |

#### Color balance

Use a 60/30/10 balance as a composition guideline: approximately 60% neutral surfaces, 30% institutional violet, and 10% semantic colors. Treat these proportions as a visual guide, not a per-component or exact measurement requirement. Keep white and light neutral surfaces predominant; use semantic colors to communicate states rather than as decoration.

## Accessibility

- **Required standard or conformance level:** To be defined
- **Assistive technologies and keyboard navigation:** To be defined
- **Validation criteria:** Verify text and interactive-state contrast for the chosen theme and color combinations; required conformance level remains to be defined.

## Assumptions and open questions

- Exact responsive breakpoints, menu interaction details, and accessibility conformance level remain to be defined; follow Bootstrap 5 conventions without inventing additional product behavior.
