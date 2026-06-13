import type { SidebarsConfig } from '@docusaurus/plugin-content-docs';

const sidebars: SidebarsConfig = {
  docsSidebar: [
    'intro',
    {
      type: 'category',
      label: 'Tutorial',
      collapsed: false,
      items: [
        'tutorial/index',
        'tutorial/create',
        'tutorial/battle',
        'tutorial/lua-badge',
      ],
    },
    {
      type: 'category',
      label: 'Guides',
      collapsed: false,
      items: [
        'guides/index',
        'guides/battle-engine',
        'guides/lua-scripting',
        'guides/plugins',
        'guides/asset-pipeline',
        'guides/rendering-hd',
        'guides/tts-narration',
        'guides/fakemons',
      ],
    },
    {
      type: 'category',
      label: 'Packages',
      collapsed: false,
      items: [
        'packages/index',
        'packages/core',
        'packages/data',
        'packages/battle',
        'packages/scripting',
        'packages/monogame',
        'packages/tools',
        'packages/plugins',
        'packages/plugins-tts',
      ],
    },
    {
      type: 'category',
      label: 'CLI pokeforge',
      items: [
        'cli/index',
        'cli/seed',
        'cli/doctor',
        'cli/fakemon',
        'cli/asset-sync',
      ],
    },
    {
      type: 'category',
      label: 'Advanced APIs',
      items: [
        'advanced/index',
        'advanced/narration-plugin',
        'advanced/fakemon-pipeline',
        'advanced/fakemon-catalog',
      ],
    },
  ],
};

export default sidebars;
