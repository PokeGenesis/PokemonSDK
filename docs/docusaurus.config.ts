import { themes as prismThemes } from 'prism-react-renderer';
import type { Config } from '@docusaurus/types';
import type * as Preset from '@docusaurus/preset-classic';

const config: Config = {
  title: 'PokemonSDK',
  tagline: 'SDK open-source C# / .NET 10 pour fan-games Pokémon',
  favicon: 'img/favicon.ico',
  url: 'https://PokeGenesis.github.io',
  baseUrl: '/PokemonSDK/',
  organizationName: 'PokeGenesis',
  projectName: 'PokemonSDK',
  trailingSlash: false,
  onBrokenLinks: 'throw',
  markdown: { hooks: { onBrokenMarkdownLinks: 'warn' } },
  i18n: { defaultLocale: 'en', locales: ['en'] },
  presets: [
    [
      'classic',
      {
        docs: {
          sidebarPath: './sidebars.ts',
          editUrl: 'https://github.com/PokeGenesis/PokemonSDK/tree/main/docs/',
        },
        blog: false,
        theme: { customCss: './src/css/custom.css' },
      } satisfies Preset.Options,
    ],
  ],
  themeConfig: {
    navbar: {
      title: 'PokemonSDK',
      items: [
        { type: 'docSidebar', sidebarId: 'docsSidebar', position: 'left', label: 'Docs' },
        { href: 'https://github.com/PokeGenesis/PokemonSDK', label: 'GitHub', position: 'right' },
      ],
    },
    footer: {
      style: 'dark',
      links: [
        {
          title: 'Docs',
          items: [{ label: 'Getting Started', to: '/docs/intro' }],
        },
        {
          title: 'Community',
          items: [{ label: 'GitHub', href: 'https://github.com/PokeGenesis/PokemonSDK' }],
        },
      ],
      copyright: `Copyright © ${new Date().getFullYear()} PokeGenesis. MIT License.`,
    },
    prism: {
      theme: prismThemes.github,
      darkTheme: prismThemes.dracula,
      additionalLanguages: ['csharp', 'lua', 'bash'],
    },
  } satisfies Preset.ThemeConfig,
};

export default config;
