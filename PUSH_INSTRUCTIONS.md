# Instructions for Pushing to GitHub

## Current Status
✅ Branch renamed to `main`
✅ All files committed locally (single commit)
✅ Ready to push

## Option 1: Simple Push (Recommended)
GitHub will handle large repositories. If your connection drops, just run the push command again - Git will resume from where it left off:

```bash
# First, create a GitHub repository at github.com, then:
git remote add origin https://github.com/YOUR_USERNAME/tank-wars.git
git push -u origin main
```

**Note:** GitHub has a 2GB warning for single files, but your largest files are under 100MB, so you're fine.

## Option 2: Push with Progress Monitoring
Monitor the upload progress:

```bash
git push -u origin main --progress
```

## Option 3: If Connection Drops
If your upload gets interrupted:
1. Simply run `git push -u origin main` again
2. Git will resume automatically
3. GitHub will accept the files that already uploaded

## Repository Size Breakdown
- **Total**: ~2.8GB (what will be uploaded)
- **Largest folders**:
  - `Assets/TankWars/`: ~2.1GB
  - `Assets/External Assets/`: ~647MB
  - Rest: ~100MB

All individual files are under GitHub's 100MB limit, so no special configuration needed.

## Troubleshooting

### If push fails due to timeout:
```bash
# Increase git buffer size
git config http.postBuffer 524288000
git push -u origin main
```

### If you want to verify before pushing:
```bash
# Check what will be pushed
git log origin/main..HEAD --stat
```

